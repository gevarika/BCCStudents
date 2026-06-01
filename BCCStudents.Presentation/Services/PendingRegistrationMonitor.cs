using BCCStudents.Application.Interfaces;
using BCCStudents.Application.Services.Sync;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using static BCCStudents.Presentation.StudentManagementForm;

namespace BCCStudents.Presentation.Services
{
    /// <summary>
    /// DownStream sync-ის შემდეგ ამოწმებს ახალ ონლაინ რეგისტრაციებს და აცხოვრებს UI-ს.
    /// </summary>
    public sealed class PendingRegistrationMonitor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IUserContext _userContext;
        private readonly PendingStudentsFormFactory _pendingStudentsFormFactory;
        private Control _invokeControl;
        private int _lastKnownCount;

        public event EventHandler<int> PendingCountChanged;

        public int CurrentCount => _lastKnownCount;

        public PendingRegistrationMonitor(
            IServiceScopeFactory scopeFactory,
            IUserContext userContext,
            PendingStudentsFormFactory pendingStudentsFormFactory)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _pendingStudentsFormFactory = pendingStudentsFormFactory ?? throw new ArgumentNullException(nameof(pendingStudentsFormFactory));
        }

        public void SetInvokeControl(Control control)
        {
            _invokeControl = control;
        }

        public void Initialize()
        {
            _lastKnownCount = GetPendingCount();
            RaisePendingCountChanged(_lastKnownCount);
        }

        public void HandleSyncCompleted(SyncStatusEventArgs args)
        {
            if (args == null || !args.Success || !args.HasPendingRegistrationChanges())
            {
                return;
            }

            var newCount = GetPendingCount();
            var newRegistrations = newCount - _lastKnownCount;

            if (newRegistrations > 0 && CanViewPendingRegistrations())
            {
                ShowNotification(newRegistrations);
            }

            if (newCount != _lastKnownCount)
            {
                _lastKnownCount = newCount;
                RaisePendingCountChanged(newCount);
            }
        }

        public void SyncLocalCount()
        {
            var count = GetPendingCount();
            _lastKnownCount = count;
            RaisePendingCountChanged(count);
        }

        private int GetPendingCount()
        {
            using var scope = _scopeFactory.CreateScope();
            var pendingService = scope.ServiceProvider.GetRequiredService<IPendingStudentService>();
            return pendingService.GetAllPending().Count;
        }

        private bool CanViewPendingRegistrations()
        {
            return _userContext.HasPermission(Permission.CanViewReports) ||
                   _userContext.HasPermission(Permission.CanManageStudents);
        }

        private void ShowNotification(int newRegistrations)
        {
            if (_invokeControl == null || _invokeControl.IsDisposed)
            {
                return;
            }

            if (_invokeControl.InvokeRequired)
            {
                _invokeControl.BeginInvoke(new Action(() => ShowNotification(newRegistrations)));
                return;
            }

            using var form = new PendingRegistrationNotificationForm(newRegistrations);
            var result = form.ShowDialog(_invokeControl.FindForm());

            if (result == DialogResult.OK && form.ShouldOpenPendingForm)
            {
                var pendingForm = _pendingStudentsFormFactory.Invoke();
                pendingForm.ShowDialog(_invokeControl.FindForm());
            }
        }

        private void RaisePendingCountChanged(int count)
        {
            PendingCountChanged?.Invoke(this, count);
        }
    }
}
