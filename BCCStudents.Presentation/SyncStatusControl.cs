using System;
using System.Drawing;
using System.Windows.Forms;
using BCCStudents.Application.Services.Sync;

namespace BCCStudents.Presentation
{
    /// <summary>
    /// áƒ™áƒáƒœáƒ¢áƒ áƒáƒšáƒ˜ áƒ¡áƒ˜áƒœáƒ¥áƒ áƒáƒœáƒ˜áƒ–áƒáƒªáƒ˜áƒ˜áƒ¡ áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ˜áƒ¡ áƒ©áƒ•áƒ”áƒœáƒ”áƒ‘áƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡ statusStrip-áƒ–áƒ”
    /// </summary>
    public class SyncStatusControl
    {
        private ToolStripStatusLabel _downStreamLabel;
        private ToolStripStatusLabel _upStreamLabel;
        private ToolStripStatusLabel _separator;
        private Control _invokeControl;

        public SyncStatusControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// áƒ˜áƒœáƒ˜áƒªáƒ˜áƒáƒšáƒ˜áƒ–áƒáƒªáƒ˜áƒ Control-áƒ˜áƒ—, áƒ áƒáƒ›áƒšáƒ˜áƒ¡ Invoke-áƒ¡áƒáƒª áƒ’áƒáƒ›áƒáƒ•áƒ˜áƒ§áƒ”áƒœáƒ”áƒ‘áƒ—
        /// </summary>
        public void SetInvokeControl(Control control)
        {
            _invokeControl = control;
        }

        private void InitializeComponent()
        {
            _downStreamLabel = new ToolStripStatusLabel
            {
                Name = "downStreamStatus",
                Text = "â†“ áƒ¡áƒ˜áƒœáƒ¥áƒ áƒáƒœáƒ˜áƒ–áƒáƒªáƒ˜áƒ: áƒ›áƒáƒšáƒáƒ“áƒ˜áƒœáƒ¨áƒ˜",
                ForeColor = Color.Gray,
                AutoSize = true
            };

            _separator = new ToolStripStatusLabel
            {
                Name = "separator",
                Text = " | ",
                AutoSize = true
            };

            _upStreamLabel = new ToolStripStatusLabel
            {
                Name = "upStreamStatus",
                Text = "â†‘ áƒ¡áƒ˜áƒœáƒ¥áƒ áƒáƒœáƒ˜áƒ–áƒáƒªáƒ˜áƒ: áƒ›áƒáƒšáƒáƒ“áƒ˜áƒœáƒ¨áƒ˜",
                ForeColor = Color.Gray,
                AutoSize = true
            };
        }

        /// <summary>
        /// áƒáƒ‘áƒ áƒ£áƒœáƒ”áƒ‘áƒ¡ ToolStripItem-áƒ”áƒ‘áƒ˜áƒ¡ áƒ¡áƒ˜áƒáƒ¡ statusStrip-áƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡
        /// </summary>
        public ToolStripItem[] GetStatusItems()
        {
            return new ToolStripItem[] { _downStreamLabel, _separator, _upStreamLabel };
        }

        /// <summary>
        /// áƒ’áƒáƒœáƒáƒáƒ®áƒšáƒ”áƒ‘áƒ¡ DownStream áƒ¡áƒ˜áƒœáƒ¥áƒ áƒáƒœáƒ˜áƒ–áƒáƒªáƒ˜áƒ˜áƒ¡ áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ¡
        /// </summary>
        public void UpdateDownStreamStatus(SyncStatusEventArgs args)
        {
            if (_invokeControl != null && _invokeControl.InvokeRequired)
            {
                _invokeControl.Invoke(new Action<SyncStatusEventArgs>(UpdateDownStreamStatus), args);
                return;
            }

            if (args.Success)
            {
                _downStreamLabel.Text = $"â†“ áƒ¡áƒ˜áƒœáƒ¥áƒ áƒáƒœáƒ˜áƒ–áƒáƒªáƒ˜áƒ: âœ… {args.RecordsSynced} áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ˜ ({args.Timestamp:HH:mm:ss})";
                _downStreamLabel.ForeColor = Color.Green;
            }
            else
            {
                var errorText = args.Errors.Count > 0 ? args.Errors[0] : "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ";
                _downStreamLabel.Text = $"â†“ áƒ¡áƒ˜áƒœáƒ¥áƒ áƒáƒœáƒ˜áƒ–áƒáƒªáƒ˜áƒ: âŒ {errorText} ({args.Timestamp:HH:mm:ss})";
                _downStreamLabel.ForeColor = Color.Red;
            }
        }

        /// <summary>
        /// áƒ’áƒáƒœáƒáƒáƒ®áƒšáƒ”áƒ‘áƒ¡ UpStream áƒ¡áƒ˜áƒœáƒ¥áƒ áƒáƒœáƒ˜áƒ–áƒáƒªáƒ˜áƒ˜áƒ¡ áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ¡
        /// </summary>
        public void UpdateUpStreamStatus(SyncStatusEventArgs args)
        {
            if (_invokeControl != null && _invokeControl.InvokeRequired)
            {
                _invokeControl.Invoke(new Action<SyncStatusEventArgs>(UpdateUpStreamStatus), args);
                return;
            }

            if (args.Success)
            {
                _upStreamLabel.Text = $"â†‘ áƒ¡áƒ˜áƒœáƒ¥áƒ áƒáƒœáƒ˜áƒ–áƒáƒªáƒ˜áƒ: âœ… {args.RecordsSynced} áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ˜ ({args.Timestamp:HH:mm:ss})";
                _upStreamLabel.ForeColor = Color.Green;
            }
            else
            {
                var errorText = args.Errors.Count > 0 ? args.Errors[0] : "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ";
                _upStreamLabel.Text = $"â†‘ áƒ¡áƒ˜áƒœáƒ¥áƒ áƒáƒœáƒ˜áƒ–áƒáƒªáƒ˜áƒ: âŒ {errorText} ({args.Timestamp:HH:mm:ss})";
                _upStreamLabel.ForeColor = Color.Red;
            }
        }

        /// <summary>
        /// áƒáƒ©áƒ•áƒ”áƒœáƒ”áƒ‘áƒ¡ áƒ¡áƒ˜áƒœáƒ¥áƒ áƒáƒœáƒ˜áƒ–áƒáƒªáƒ˜áƒ˜áƒ¡ áƒžáƒ áƒáƒªáƒ”áƒ¡áƒ¡
        /// </summary>
        public void ShowSyncing(string syncType)
        {
            if (_invokeControl != null && _invokeControl.InvokeRequired)
            {
                _invokeControl.Invoke(new Action<string>(ShowSyncing), syncType);
                return;
            }

            if (syncType == "DownStream")
            {
                _downStreamLabel.Text = "â†“ áƒ¡áƒ˜áƒœáƒ¥áƒ áƒáƒœáƒ˜áƒ–áƒáƒªáƒ˜áƒ: ðŸ”„ áƒ›áƒ˜áƒ›áƒ“áƒ˜áƒœáƒáƒ áƒ”...";
                _downStreamLabel.ForeColor = Color.Orange;
            }
            else if (syncType == "UpStream")
            {
                _upStreamLabel.Text = "â†‘ áƒ¡áƒ˜áƒœáƒ¥áƒ áƒáƒœáƒ˜áƒ–áƒáƒªáƒ˜áƒ: ðŸ”„ áƒ›áƒ˜áƒ›áƒ“áƒ˜áƒœáƒáƒ áƒ”...";
                _upStreamLabel.ForeColor = Color.Orange;
            }
        }
    }
}


