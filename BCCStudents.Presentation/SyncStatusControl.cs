using BCCStudents.Application.Services.Sync;

namespace BCCStudents.Presentation
{
    /// <summary>
    /// კონტროლი სინქრონიზაციის სტატუსის ჩვენებისთვის statusStrip-ზე
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
        /// ინიციალიზაცია Control-ით, რომლის Invoke-საც გამოვიყენებთ
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
                Text = "↓ სინქრონიზაცია: მოლოდინში",
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
                Text = "↑ სინქრონიზაცია: მოლოდინში",
                ForeColor = Color.Gray,
                AutoSize = true
            };
        }

        /// <summary>
        /// აბრუნებს ToolStripItem-ების სიას statusStrip-ისთვის
        /// </summary>
        public ToolStripItem[] GetStatusItems()
        {
            return new ToolStripItem[] { _downStreamLabel, _separator, _upStreamLabel };
        }

        /// <summary>
        /// განაახლებს DownStream სინქრონიზაციის სტატუსს
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
                _downStreamLabel.Text = $"↓ სინქრონიზაცია: OK {args.RecordsSynced} ჩანაწერი ({args.Timestamp:HH:mm:ss})";
                _downStreamLabel.ForeColor = Color.Green;
            }
            else
            {
                _downStreamLabel.Text = $"↓ ჩამოტვირთვის შეცდომა ({args.Timestamp:HH:mm:ss})";
                _downStreamLabel.ForeColor = Color.Red;
            }
        }

        /// <summary>
        /// განაახლებს UpStream სინქრონიზაციის სტატუსს
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
                _upStreamLabel.Text = $"↑ სინქრონიზაცია: OK {args.RecordsSynced} ჩანაწერი ({args.Timestamp:HH:mm:ss})";
                _upStreamLabel.ForeColor = Color.Green;
            }
            else
            {
                _upStreamLabel.Text = $"↑ ატვირთვის შეცდომა ({args.Timestamp:HH:mm:ss})";
                _upStreamLabel.ForeColor = Color.Red;
            }
        }

        /// <summary>
        /// აჩვენებს სინქრონიზაციის პროცესს
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
                _downStreamLabel.Text = "↓ სინქრონიზაცია: მიმდინარეობს...";
                _downStreamLabel.ForeColor = Color.Orange;
            }
            else if (syncType == "UpStream")
            {
                _upStreamLabel.Text = "↑ სინქრონიზაცია: მიმდინარეობს...";
                _upStreamLabel.ForeColor = Color.Orange;
            }
        }
    }
}


