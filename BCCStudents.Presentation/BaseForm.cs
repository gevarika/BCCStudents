using System;
using System.Windows.Forms;
using BCCStudents.Application.Services.Sync;
using BCCStudents.Application.Interfaces;

namespace BCCStudents.Presentation
{
    /// <summary>
    /// Base Form áƒ áƒáƒ›áƒ”áƒšáƒ˜áƒª áƒ¨áƒ”áƒ˜áƒªáƒáƒ•áƒ¡ áƒ¡áƒ˜áƒœáƒ¥áƒ áƒáƒœáƒ˜áƒ–áƒáƒªáƒ˜áƒ˜áƒ¡ áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ˜áƒ¡ áƒ™áƒáƒœáƒ¢áƒ áƒáƒšáƒ¡
    /// </summary>
    public partial class BaseForm : Form
    {
        protected SyncStatusControl SyncStatusControl { get; private set; }
        protected StatusStrip StatusStrip { get; private set; }

        /// <summary>
        /// áƒ˜áƒœáƒ˜áƒªáƒ˜áƒáƒšáƒ˜áƒ–áƒáƒªáƒ˜áƒ áƒ¡áƒ˜áƒœáƒ¥áƒ áƒáƒœáƒ˜áƒ–áƒáƒªáƒ˜áƒ˜áƒ¡ áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ˜áƒ¡ áƒ™áƒáƒœáƒ¢áƒ áƒáƒšáƒ˜áƒ—
        /// </summary>
        protected void InitializeSyncStatus(IDownStreamSyncManager downStreamManager, IUpStreamSyncManager upStreamManager)
        {
            // áƒ•áƒ¥áƒ›áƒœáƒ˜áƒ— StatusStrip-áƒ¡ áƒ—áƒ£ áƒáƒ  áƒáƒ áƒ¡áƒ”áƒ‘áƒáƒ‘áƒ¡
            if (StatusStrip == null)
            {
                StatusStrip = new StatusStrip
                {
                    Name = "statusStrip",
                    Dock = DockStyle.Bottom
                };
                this.Controls.Add(StatusStrip);
            }

            // áƒ•áƒ¥áƒ›áƒœáƒ˜áƒ— SyncStatusControl-áƒ¡
            SyncStatusControl = new SyncStatusControl();
            // áƒ•áƒáƒ§áƒ”áƒœáƒ”áƒ‘áƒ— Control-áƒ¡ Invoke-áƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡
            SyncStatusControl.SetInvokeControl(StatusStrip);
            var statusItems = SyncStatusControl.GetStatusItems();

            // áƒ•áƒáƒ›áƒáƒ¢áƒ”áƒ‘áƒ— statusStrip-áƒ–áƒ”
            foreach (var item in statusItems)
            {
                StatusStrip.Items.Add(item);
            }

            // Event handlers-áƒ˜áƒ¡ áƒ“áƒáƒ›áƒáƒ¢áƒ”áƒ‘áƒ
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


