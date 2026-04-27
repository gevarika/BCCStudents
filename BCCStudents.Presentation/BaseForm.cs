using BCCStudents.Application.Interfaces;

namespace BCCStudents.Presentation
{
    /// <summary>
    /// BaseForm - საერთო ბაზური ფორმა, რომელიც შეიცავს სინქრონიზაციის სტატუსის კონტროლს.
    /// </summary>
    public partial class BaseForm : Form
    {
        protected SyncStatusControl SyncStatusControl { get; private set; }
        protected StatusStrip StatusStrip { get; private set; }

        /// <summary>
        /// სინქრონიზაციის სტატუსის კონტროლის ინიციალიზაცია.
        /// </summary>
        protected void InitializeSyncStatus(IDownStreamSyncManager downStreamManager, IUpStreamSyncManager upStreamManager)
        {
            // თუ StatusStrip ჯერ არ არის შექმნილი, ვქმნით
            if (StatusStrip == null)
            {
                StatusStrip = new StatusStrip
                {
                    Name = "statusStrip",
                    Dock = DockStyle.Bottom
                };
                this.Controls.Add(StatusStrip);
            }

            // ვქმნით SyncStatusControl-ს
            SyncStatusControl = new SyncStatusControl();
            // ვუთითებთ Control-ს, რომელსაც გამოიყენებს Invoke-სთვის
            SyncStatusControl.SetInvokeControl(StatusStrip);
            var statusItems = SyncStatusControl.GetStatusItems();

            // ვამატებთ მიღებულ StatusItem-ებს StatusStrip-ში
            foreach (var item in statusItems)
            {
                StatusStrip.Items.Add(item);
            }

            // ვამაგრებთ Event handler-ებს DownStream / UpStream სინქისთვის
            if (downStreamManager != null)
            {
                downStreamManager.SyncCompleted += (sender, args) =>
                {
                    SyncStatusControl.UpdateDownStreamStatus(args);
                };
            }

            if (upStreamManager != null)
            {
                upStreamManager.SyncCompleted += (sender, args) =>
                {
                    SyncStatusControl.UpdateUpStreamStatus(args);
                };
            }
        }
    }
}


