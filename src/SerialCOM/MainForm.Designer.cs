namespace SerialCOM;

partial class MainForm
{
    /// <summary>Required designer variable.</summary>
    private System.ComponentModel.IContainer components = null!;

    private System.Windows.Forms.Timer _reconnectTimer = null!;
    private System.Windows.Forms.Timer _statusTimer = null!;
    private ToolTip _tips = null!;

    private MenuStrip menuStrip = null!;
    private ToolStripMenuItem mnuFile = null!;
    private ToolStripMenuItem miSaveLog = null!;
    private ToolStripMenuItem miClearLog = null!;
    private ToolStripMenuItem miExit = null!;
    private ToolStripMenuItem mnuView = null!;
    private ToolStripMenuItem mnuSidePanel = null!;
    private ToolStripMenuItem mnuAlwaysOnTop = null!;
    private ToolStripMenuItem mnuWordWrap = null!;
    private ToolStripMenuItem mnuAutoScroll = null!;
    private ToolStripMenuItem mnuTimestamps = null!;
    private ToolStripMenuItem mnuDarkTerminal = null!;
    private ToolStripMenuItem mnuColorize = null!;
    private ToolStripMenuItem mnuHelp = null!;
    private ToolStripMenuItem miAbout = null!;

    private ToolStrip toolStrip = null!;
    private ToolStripButton tbtnPanel = null!;
    private ToolStripButton tbtnRefresh = null!;
    private ToolStripButton tbtnClear = null!;
    private ToolStripButton tbtnBreak = null!;
    private ToolStripButton tbtnSaveLog = null!;

    private StatusStrip statusStrip = null!;
    private ToolStripStatusLabel toolStatus = null!;
    private ToolStripStatusLabel toolRx = null!;
    private ToolStripStatusLabel toolTx = null!;
    private ToolStripStatusLabel toolError = null!;

    private SplitContainer split = null!;
    private Panel leftPanel = null!;
    private GroupBox grpConnection = null!;
    private GroupBox grpTx = null!;
    private GroupBox grpView = null!;

    private TableLayoutPanel tblConnection = null!;
    private Label lblPort = null!;
    private TableLayoutPanel tblPort = null!;
    private ComboBox cmbPort = null!;
    private Button btnPortRefresh = null!;
    private Label lblBaud = null!;
    private ComboBox cmbBaud = null!;
    private Label lblDataBits = null!;
    private ComboBox cmbDataBits = null!;
    private Label lblParity = null!;
    private ComboBox cmbParity = null!;
    private Label lblStopBits = null!;
    private ComboBox cmbStopBits = null!;
    private Label lblFlow = null!;
    private ComboBox cmbHandshake = null!;
    private FlowLayoutPanel flpDtrRts = null!;
    private CheckBox chkDtr = null!;
    private CheckBox chkRts = null!;
    private Button btnConnect = null!;
    private FlowLayoutPanel flpLed = null!;
    private Label lblLedDot = null!;
    private Label lblLedText = null!;
    private Label lblConnHint = null!;

    private TableLayoutPanel tblTx = null!;
    private Label lblTxMode = null!;
    private ComboBox cmbTxMode = null!;
    private Label lblLineEnding = null!;
    private ComboBox cmbLineEnding = null!;
    private Label lblEncoding = null!;
    private ComboBox cmbEncoding = null!;

    private TableLayoutPanel tblView = null!;
    private Label lblDisplay = null!;
    private ComboBox cmbDisplay = null!;
    private FlowLayoutPanel flpViewChecks = null!;
    private CheckBox chkEcho = null!;
    private CheckBox chkTimestamp = null!;
    private CheckBox chkAutoscroll = null!;
    private CheckBox chkWrap = null!;
    private CheckBox chkDarkTerminal = null!;
    private CheckBox chkColorize = null!;
    private CheckBox chkLogging = null!;
    private TableLayoutPanel tblLogPath = null!;
    private TextBox txtLogPath = null!;
    private Button btnBrowseLog = null!;
    private FlowLayoutPanel flpSettingsButtons = null!;
    private Button btnSaveSettings = null!;
    private Button btnLoadSettings = null!;
    private Button btnDefaultSettings = null!;

    private FlowLayoutPanel optsBar = null!;
    private CheckBox chkAutoReconnect = null!;
    private Label lblHint = null!;
    private TableLayoutPanel sendBar = null!;
    private TextBox txtSend = null!;
    private Button btnSend = null!;
    private Button btnSendFile = null!;
    private FlickerFreeRichTextBox rtbTerminal = null!;

    /// <summary>Clean up any resources being used.</summary>
    /// <param name="disposing">true if managed resources should be disposed.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
            _serial.Dispose();
            _log.Dispose();
        }
        base.Dispose(disposing);
    }

    /// <summary>Required method for Designer support.</summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        _reconnectTimer = new System.Windows.Forms.Timer(components);
        _statusTimer = new System.Windows.Forms.Timer(components);
        _tips = new ToolTip(components);

        menuStrip = new MenuStrip();
        mnuFile = new ToolStripMenuItem();
        miSaveLog = new ToolStripMenuItem();
        miClearLog = new ToolStripMenuItem();
        miExit = new ToolStripMenuItem();
        mnuView = new ToolStripMenuItem();
        mnuSidePanel = new ToolStripMenuItem();
        mnuAlwaysOnTop = new ToolStripMenuItem();
        mnuWordWrap = new ToolStripMenuItem();
        mnuAutoScroll = new ToolStripMenuItem();
        mnuTimestamps = new ToolStripMenuItem();
        mnuDarkTerminal = new ToolStripMenuItem();
        mnuColorize = new ToolStripMenuItem();
        mnuHelp = new ToolStripMenuItem();
        miAbout = new ToolStripMenuItem();

        toolStrip = new ToolStrip();
        tbtnPanel = new ToolStripButton();
        tbtnRefresh = new ToolStripButton();
        tbtnClear = new ToolStripButton();
        tbtnBreak = new ToolStripButton();
        tbtnSaveLog = new ToolStripButton();

        statusStrip = new StatusStrip();
        toolStatus = new ToolStripStatusLabel();
        toolRx = new ToolStripStatusLabel();
        toolTx = new ToolStripStatusLabel();
        toolError = new ToolStripStatusLabel();

        split = new SplitContainer();
        leftPanel = new Panel();
        grpConnection = new GroupBox();
        grpTx = new GroupBox();
        grpView = new GroupBox();

        tblConnection = new TableLayoutPanel();
        lblPort = new Label();
        tblPort = new TableLayoutPanel();
        cmbPort = new ComboBox();
        btnPortRefresh = new Button();
        lblBaud = new Label();
        cmbBaud = new ComboBox();
        lblDataBits = new Label();
        cmbDataBits = new ComboBox();
        lblParity = new Label();
        cmbParity = new ComboBox();
        lblStopBits = new Label();
        cmbStopBits = new ComboBox();
        lblFlow = new Label();
        cmbHandshake = new ComboBox();
        flpDtrRts = new FlowLayoutPanel();
        chkDtr = new CheckBox();
        chkRts = new CheckBox();
        btnConnect = new Button();
        flpLed = new FlowLayoutPanel();
        lblLedDot = new Label();
        lblLedText = new Label();
        lblConnHint = new Label();

        tblTx = new TableLayoutPanel();
        lblTxMode = new Label();
        cmbTxMode = new ComboBox();
        lblLineEnding = new Label();
        cmbLineEnding = new ComboBox();
        lblEncoding = new Label();
        cmbEncoding = new ComboBox();

        tblView = new TableLayoutPanel();
        lblDisplay = new Label();
        cmbDisplay = new ComboBox();
        flpViewChecks = new FlowLayoutPanel();
        chkEcho = new CheckBox();
        chkTimestamp = new CheckBox();
        chkAutoscroll = new CheckBox();
        chkWrap = new CheckBox();
        chkDarkTerminal = new CheckBox();
        chkColorize = new CheckBox();
        chkLogging = new CheckBox();
        tblLogPath = new TableLayoutPanel();
        txtLogPath = new TextBox();
        btnBrowseLog = new Button();
        flpSettingsButtons = new FlowLayoutPanel();
        btnSaveSettings = new Button();
        btnLoadSettings = new Button();
        btnDefaultSettings = new Button();

        optsBar = new FlowLayoutPanel();
        chkAutoReconnect = new CheckBox();
        lblHint = new Label();
        sendBar = new TableLayoutPanel();
        txtSend = new TextBox();
        btnSend = new Button();
        btnSendFile = new Button();
        rtbTerminal = new FlickerFreeRichTextBox();

        SuspendLayout();
        menuStrip.SuspendLayout();
        toolStrip.SuspendLayout();
        statusStrip.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)split).BeginInit();
        split.Panel1.SuspendLayout();
        split.Panel2.SuspendLayout();
        split.SuspendLayout();
        leftPanel.SuspendLayout();
        grpConnection.SuspendLayout();
        grpTx.SuspendLayout();
        grpView.SuspendLayout();
        tblConnection.SuspendLayout();
        tblPort.SuspendLayout();
        flpDtrRts.SuspendLayout();
        flpLed.SuspendLayout();
        tblTx.SuspendLayout();
        tblView.SuspendLayout();
        flpViewChecks.SuspendLayout();
        tblLogPath.SuspendLayout();
        flpSettingsButtons.SuspendLayout();
        optsBar.SuspendLayout();
        sendBar.SuspendLayout();

        //
        // MainForm
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(1000, 650);
        MinimumSize = new Size(850, 550);
        StartPosition = FormStartPosition.CenterScreen;
        Text = "SerialCOM";
        KeyPreview = true;

        //
        // timers + tips
        //
        _reconnectTimer.Interval = 2000;
        _reconnectTimer.Tick += ReconnectTimer_Tick;
        _statusTimer.Interval = 5000;
        _statusTimer.Tick += StatusTimer_Tick;
        _tips.AutoPopDelay = 5000;
        _tips.InitialDelay = 500;
        _tips.ReshowDelay = 100;

        //
        // menuStrip
        //
        menuStrip.Dock = DockStyle.Top;
        menuStrip.Items.AddRange(new ToolStripItem[] { mnuFile, mnuView, mnuHelp });
        menuStrip.Location = new Point(0, 0);
        menuStrip.Name = "menuStrip";
        menuStrip.Size = new Size(1000, 24);
        menuStrip.TabIndex = 0;

        mnuFile.DropDownItems.AddRange(new ToolStripItem[] { miSaveLog, miClearLog, new ToolStripSeparator(), miExit });
        mnuFile.Name = "mnuFile";
        mnuFile.Text = "&File";

        miSaveLog.Name = "miSaveLog";
        miSaveLog.ShortcutKeys = Keys.Control | Keys.S;
        miSaveLog.Text = "&Save log as…";
        miSaveLog.Click += MiSaveLog_Click;

        miClearLog.Name = "miClearLog";
        miClearLog.ShortcutKeys = Keys.Control | Keys.L;
        miClearLog.Text = "&Clear terminal";
        miClearLog.Click += MiClearLog_Click;

        miExit.Name = "miExit";
        miExit.ShortcutKeys = Keys.Alt | Keys.F4;
        miExit.Text = "E&xit";
        miExit.Click += MiExit_Click;

        mnuView.DropDownItems.AddRange(new ToolStripItem[]
        {
            mnuSidePanel, new ToolStripSeparator(), mnuAlwaysOnTop, mnuWordWrap,
            mnuAutoScroll, mnuTimestamps, new ToolStripSeparator(), mnuDarkTerminal, mnuColorize,
        });
        mnuView.Name = "mnuView";
        mnuView.Text = "&View";

        mnuSidePanel.CheckOnClick = true;
        mnuSidePanel.Checked = true;
        mnuSidePanel.Name = "mnuSidePanel";
        mnuSidePanel.ShortcutKeys = Keys.F9;
        mnuSidePanel.Text = "Connection &panel";
        mnuSidePanel.Click += MnuSidePanel_Click;

        mnuAlwaysOnTop.CheckOnClick = true;
        mnuAlwaysOnTop.Name = "mnuAlwaysOnTop";
        mnuAlwaysOnTop.Text = "&Always on top";
        mnuAlwaysOnTop.Click += MnuAlwaysOnTop_Click;

        mnuWordWrap.CheckOnClick = true;
        mnuWordWrap.Checked = true;
        mnuWordWrap.Name = "mnuWordWrap";
        mnuWordWrap.Text = "&Word wrap";
        mnuWordWrap.Click += MnuWordWrap_Click;

        mnuAutoScroll.CheckOnClick = true;
        mnuAutoScroll.Checked = true;
        mnuAutoScroll.Name = "mnuAutoScroll";
        mnuAutoScroll.Text = "Auto-&scroll";
        mnuAutoScroll.Click += MnuAutoScroll_Click;

        mnuTimestamps.CheckOnClick = true;
        mnuTimestamps.Name = "mnuTimestamps";
        mnuTimestamps.Text = "&Timestamps";
        mnuTimestamps.Click += MnuTimestamps_Click;

        mnuDarkTerminal.CheckOnClick = true;
        mnuDarkTerminal.Name = "mnuDarkTerminal";
        mnuDarkTerminal.Text = "&Dark terminal";
        mnuDarkTerminal.Click += MnuDarkTerminal_Click;

        mnuColorize.CheckOnClick = true;
        mnuColorize.Checked = true;
        mnuColorize.Name = "mnuColorize";
        mnuColorize.Text = "Co&lorize RX/TX";
        mnuColorize.Click += MnuColorize_Click;

        mnuHelp.DropDownItems.AddRange(new ToolStripItem[] { miAbout });
        mnuHelp.Name = "mnuHelp";
        mnuHelp.Text = "&Help";

        miAbout.Name = "miAbout";
        miAbout.Text = "&About SerialCOM";
        miAbout.Click += MiAbout_Click;

        //
        // toolStrip
        //
        toolStrip.Dock = DockStyle.Top;
        toolStrip.GripStyle = ToolStripGripStyle.Hidden;
        toolStrip.Items.AddRange(new ToolStripItem[]
        {
            tbtnPanel, new ToolStripSeparator(), tbtnRefresh, tbtnClear,
            tbtnBreak, new ToolStripSeparator(), tbtnSaveLog,
        });
        toolStrip.Location = new Point(0, 24);
        toolStrip.Name = "toolStrip";
        toolStrip.Size = new Size(1000, 25);
        toolStrip.TabIndex = 1;

        tbtnPanel.AutoSize = true;
        tbtnPanel.CheckOnClick = true;
        tbtnPanel.Checked = true;
        tbtnPanel.DisplayStyle = ToolStripItemDisplayStyle.Text;
        tbtnPanel.Name = "tbtnPanel";
        tbtnPanel.Text = "☰ Panel (F9)";
        tbtnPanel.ToolTipText = "Show/hide the connection panel (F9)";
        tbtnPanel.Click += TbtnPanel_Click;

        tbtnRefresh.AutoSize = true;
        tbtnRefresh.DisplayStyle = ToolStripItemDisplayStyle.Text;
        tbtnRefresh.Name = "tbtnRefresh";
        tbtnRefresh.Text = "⟳ Refresh (F5)";
        tbtnRefresh.ToolTipText = "Rescan COM ports (F5)";
        tbtnRefresh.Click += TbtnRefresh_Click;

        tbtnClear.AutoSize = true;
        tbtnClear.DisplayStyle = ToolStripItemDisplayStyle.Text;
        tbtnClear.Name = "tbtnClear";
        tbtnClear.Text = "✕ Clear (Ctrl+L)";
        tbtnClear.ToolTipText = "Clear the terminal (Ctrl+L)";
        tbtnClear.Click += TbtnClear_Click;

        tbtnBreak.AutoSize = true;
        tbtnBreak.DisplayStyle = ToolStripItemDisplayStyle.Text;
        tbtnBreak.Name = "tbtnBreak";
        tbtnBreak.Text = "⚡ Break";
        tbtnBreak.ToolTipText = "Hold the BREAK condition for 500 ms";
        tbtnBreak.Click += TbtnBreak_Click;

        tbtnSaveLog.AutoSize = true;
        tbtnSaveLog.DisplayStyle = ToolStripItemDisplayStyle.Text;
        tbtnSaveLog.Name = "tbtnSaveLog";
        tbtnSaveLog.Text = "💾 Save log… (Ctrl+S)";
        tbtnSaveLog.ToolTipText = "Copy the terminal buffer to a text file (Ctrl+S)";
        tbtnSaveLog.Click += TbtnSaveLog_Click;

        //
        // statusStrip
        //
        statusStrip.Items.AddRange(new ToolStripItem[] { toolStatus, toolRx, toolTx, toolError });
        statusStrip.Location = new Point(0, 628);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(1000, 22);
        statusStrip.SizingGrip = true;
        statusStrip.TabIndex = 2;

        toolStatus.Name = "toolStatus";
        toolStatus.Spring = true;
        toolStatus.Text = "Disconnected";
        toolStatus.TextAlign = ContentAlignment.MiddleLeft;

        toolRx.AutoSize = true;
        toolRx.BorderSides = ToolStripStatusLabelBorderSides.Left;
        toolRx.Name = "toolRx";
        toolRx.Text = "RX: 0";

        toolTx.AutoSize = true;
        toolTx.BorderSides = ToolStripStatusLabelBorderSides.Left;
        toolTx.Name = "toolTx";
        toolTx.Text = "TX: 0";

        toolError.ForeColor = Color.Firebrick;
        toolError.Name = "toolError";
        toolError.TextAlign = ContentAlignment.MiddleLeft;

        //
        // split
        //
        split.Dock = DockStyle.Fill;
        split.FixedPanel = FixedPanel.Panel1;
        split.Location = new Point(0, 49);
        split.Name = "split";
        split.Panel1MinSize = 220;
        split.Panel2MinSize = 300;
        split.Size = new Size(1000, 579);
        split.SplitterDistance = 280;
        split.TabIndex = 3;

        //
        // leftPanel
        //
        leftPanel.AutoScroll = true;
        leftPanel.Dock = DockStyle.Fill;
        leftPanel.Location = new Point(0, 0);
        leftPanel.Name = "leftPanel";
        leftPanel.Padding = new Padding(8);
        leftPanel.Size = new Size(280, 579);
        leftPanel.TabIndex = 0;
        // All groups Dock.Top: add bottom-most first so Connection ends up on top.
        leftPanel.Controls.Add(grpView);
        leftPanel.Controls.Add(grpTx);
        leftPanel.Controls.Add(grpConnection);

        //
        // grpConnection
        //
        grpConnection.AutoSize = true;
        grpConnection.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        grpConnection.Dock = DockStyle.Top;
        grpConnection.Margin = new Padding(0, 0, 0, 8);
        grpConnection.Name = "grpConnection";
        grpConnection.Padding = new Padding(8);
        grpConnection.Size = new Size(264, 320);
        grpConnection.TabIndex = 0;
        grpConnection.Text = "Connection";
        // Dock.Top stacks last-added on top — add bottom-most first.
        grpConnection.Controls.Add(lblConnHint);
        grpConnection.Controls.Add(flpLed);
        grpConnection.Controls.Add(btnConnect);
        grpConnection.Controls.Add(flpDtrRts);
        grpConnection.Controls.Add(tblConnection);

        //
        // tblConnection — label 75px + control Fill
        //
        tblConnection.AutoSize = true;
        tblConnection.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        tblConnection.ColumnCount = 2;
        tblConnection.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75F));
        tblConnection.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tblConnection.Dock = DockStyle.Top;
        tblConnection.Location = new Point(8, 23);
        tblConnection.Name = "tblConnection";
        tblConnection.Padding = new Padding(0, 0, 0, 4);
        tblConnection.RowCount = 6;
        tblConnection.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblConnection.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblConnection.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblConnection.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblConnection.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblConnection.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblConnection.Size = new Size(248, 174);
        tblConnection.TabIndex = 0;
        tblConnection.Controls.Add(lblPort, 0, 0);
        tblConnection.Controls.Add(tblPort, 1, 0);
        tblConnection.Controls.Add(lblBaud, 0, 1);
        tblConnection.Controls.Add(cmbBaud, 1, 1);
        tblConnection.Controls.Add(lblDataBits, 0, 2);
        tblConnection.Controls.Add(cmbDataBits, 1, 2);
        tblConnection.Controls.Add(lblParity, 0, 3);
        tblConnection.Controls.Add(cmbParity, 1, 3);
        tblConnection.Controls.Add(lblStopBits, 0, 4);
        tblConnection.Controls.Add(cmbStopBits, 1, 4);
        tblConnection.Controls.Add(lblFlow, 0, 5);
        tblConnection.Controls.Add(cmbHandshake, 1, 5);

        lblPort.Anchor = AnchorStyles.Left;
        lblPort.AutoSize = true;
        lblPort.Location = new Point(3, 9);
        lblPort.Margin = new Padding(3, 6, 3, 0);
        lblPort.Name = "lblPort";
        lblPort.Size = new Size(28, 15);
        lblPort.TabIndex = 0;
        lblPort.Text = "Port";

        //
        // tblPort — combo Fill + Refresh 75px
        //
        tblPort.ColumnCount = 2;
        tblPort.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tblPort.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75F));
        tblPort.Dock = DockStyle.Fill;
        tblPort.Location = new Point(78, 3);
        tblPort.Margin = new Padding(0);
        tblPort.Name = "tblPort";
        tblPort.RowCount = 1;
        tblPort.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblPort.Size = new Size(170, 29);
        tblPort.TabIndex = 1;
        tblPort.Controls.Add(cmbPort, 0, 0);
        tblPort.Controls.Add(btnPortRefresh, 1, 0);

        cmbPort.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        cmbPort.Dock = DockStyle.Fill;
        cmbPort.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbPort.FormattingEnabled = true;
        cmbPort.Location = new Point(0, 3);
        cmbPort.Margin = new Padding(0, 3, 3, 3);
        cmbPort.Name = "cmbPort";
        cmbPort.Size = new Size(92, 23);
        cmbPort.TabIndex = 0;
        _tips.SetToolTip(cmbPort, "Select a COM port");
        cmbPort.DropDownClosed += CmbPort_DropDownClosed;

        btnPortRefresh.Anchor = AnchorStyles.Right;
        btnPortRefresh.Location = new Point(95, 3);
        btnPortRefresh.Margin = new Padding(3, 3, 0, 3);
        btnPortRefresh.Name = "btnPortRefresh";
        btnPortRefresh.Size = new Size(75, 23);
        btnPortRefresh.TabIndex = 1;
        btnPortRefresh.Text = "Refresh";
        _tips.SetToolTip(btnPortRefresh, "Rescan COM ports (F5)");
        btnPortRefresh.Click += BtnPortRefresh_Click;

        lblBaud.Anchor = AnchorStyles.Left;
        lblBaud.AutoSize = true;
        lblBaud.Location = new Point(3, 38);
        lblBaud.Margin = new Padding(3, 6, 3, 0);
        lblBaud.Name = "lblBaud";
        lblBaud.Size = new Size(34, 15);
        lblBaud.TabIndex = 0;
        lblBaud.Text = "Baud";

        cmbBaud.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        cmbBaud.Dock = DockStyle.Fill;
        cmbBaud.DropDownStyle = ComboBoxStyle.DropDown;
        cmbBaud.FormattingEnabled = true;
        cmbBaud.Items.AddRange(new object[] { "9600", "19200", "38400", "57600", "115200", "230400", "460800", "921600" });
        cmbBaud.Location = new Point(78, 32);
        cmbBaud.Margin = new Padding(0, 3, 0, 3);
        cmbBaud.Name = "cmbBaud";
        cmbBaud.Size = new Size(170, 23);
        cmbBaud.TabIndex = 2;
        cmbBaud.Text = "115200";

        lblDataBits.Anchor = AnchorStyles.Left;
        lblDataBits.AutoSize = true;
        lblDataBits.Location = new Point(3, 67);
        lblDataBits.Margin = new Padding(3, 6, 3, 0);
        lblDataBits.Name = "lblDataBits";
        lblDataBits.Size = new Size(55, 15);
        lblDataBits.TabIndex = 0;
        lblDataBits.Text = "Data bits";

        cmbDataBits.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        cmbDataBits.Dock = DockStyle.Fill;
        cmbDataBits.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbDataBits.FormattingEnabled = true;
        cmbDataBits.Items.AddRange(new object[] { "5", "6", "7", "8" });
        cmbDataBits.Location = new Point(78, 61);
        cmbDataBits.Margin = new Padding(0, 3, 0, 3);
        cmbDataBits.Name = "cmbDataBits";
        cmbDataBits.Size = new Size(170, 23);
        cmbDataBits.TabIndex = 3;

        lblParity.Anchor = AnchorStyles.Left;
        lblParity.AutoSize = true;
        lblParity.Location = new Point(3, 96);
        lblParity.Margin = new Padding(3, 6, 3, 0);
        lblParity.Name = "lblParity";
        lblParity.Size = new Size(37, 15);
        lblParity.TabIndex = 0;
        lblParity.Text = "Parity";

        cmbParity.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        cmbParity.Dock = DockStyle.Fill;
        cmbParity.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbParity.FormattingEnabled = true;
        cmbParity.Items.AddRange(new object[] { "None", "Odd", "Even", "Mark", "Space" });
        cmbParity.Location = new Point(78, 90);
        cmbParity.Margin = new Padding(0, 3, 0, 3);
        cmbParity.Name = "cmbParity";
        cmbParity.Size = new Size(170, 23);
        cmbParity.TabIndex = 4;

        lblStopBits.Anchor = AnchorStyles.Left;
        lblStopBits.AutoSize = true;
        lblStopBits.Location = new Point(3, 125);
        lblStopBits.Margin = new Padding(3, 6, 3, 0);
        lblStopBits.Name = "lblStopBits";
        lblStopBits.Size = new Size(54, 15);
        lblStopBits.TabIndex = 0;
        lblStopBits.Text = "Stop bits";

        cmbStopBits.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        cmbStopBits.Dock = DockStyle.Fill;
        cmbStopBits.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbStopBits.FormattingEnabled = true;
        cmbStopBits.Items.AddRange(new object[] { "One", "OnePointFive", "Two" });
        cmbStopBits.Location = new Point(78, 119);
        cmbStopBits.Margin = new Padding(0, 3, 0, 3);
        cmbStopBits.Name = "cmbStopBits";
        cmbStopBits.Size = new Size(170, 23);
        cmbStopBits.TabIndex = 5;

        lblFlow.Anchor = AnchorStyles.Left;
        lblFlow.AutoSize = true;
        lblFlow.Location = new Point(3, 154);
        lblFlow.Margin = new Padding(3, 6, 3, 0);
        lblFlow.Name = "lblFlow";
        lblFlow.Size = new Size(32, 15);
        lblFlow.TabIndex = 0;
        lblFlow.Text = "Flow";

        cmbHandshake.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        cmbHandshake.Dock = DockStyle.Fill;
        cmbHandshake.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbHandshake.FormattingEnabled = true;
        cmbHandshake.Items.AddRange(new object[] { "None", "XOnXOff", "RequestToSend", "RequestToSendXOnXOff" });
        cmbHandshake.Location = new Point(78, 148);
        cmbHandshake.Margin = new Padding(0, 3, 0, 3);
        cmbHandshake.Name = "cmbHandshake";
        cmbHandshake.Size = new Size(170, 23);
        cmbHandshake.TabIndex = 6;

        //
        // flpDtrRts
        //
        flpDtrRts.AutoSize = true;
        flpDtrRts.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flpDtrRts.Dock = DockStyle.Top;
        flpDtrRts.Location = new Point(8, 197);
        flpDtrRts.Name = "flpDtrRts";
        flpDtrRts.Padding = new Padding(0, 6, 0, 0);
        flpDtrRts.Size = new Size(248, 31);
        flpDtrRts.TabIndex = 1;
        flpDtrRts.WrapContents = true;
        flpDtrRts.Controls.Add(chkDtr);
        flpDtrRts.Controls.Add(chkRts);

        chkDtr.AutoSize = true;
        chkDtr.Location = new Point(3, 9);
        chkDtr.Name = "chkDtr";
        chkDtr.Size = new Size(84, 19);
        chkDtr.TabIndex = 7;
        chkDtr.Text = "DTR enable";
        _tips.SetToolTip(chkDtr, "Data Terminal Ready — stays adjustable while connected");
        chkDtr.CheckedChanged += ChkDtr_CheckedChanged;

        chkRts.AutoSize = true;
        chkRts.Location = new Point(93, 9);
        chkRts.Name = "chkRts";
        chkRts.Size = new Size(82, 19);
        chkRts.TabIndex = 8;
        chkRts.Text = "RTS enable";
        _tips.SetToolTip(chkRts, "Request To Send — stays adjustable while connected");
        chkRts.CheckedChanged += ChkRts_CheckedChanged;

        //
        // btnConnect
        //
        btnConnect.Dock = DockStyle.Top;
        btnConnect.Height = 32;
        btnConnect.Location = new Point(8, 228);
        btnConnect.Margin = new Padding(3, 6, 3, 3);
        btnConnect.Name = "btnConnect";
        btnConnect.Size = new Size(248, 32);
        btnConnect.TabIndex = 9;
        btnConnect.Text = "▶  Connect";
        _tips.SetToolTip(btnConnect, "Open/close the selected COM port");
        btnConnect.Click += BtnConnect_Click;

        //
        // flpLed
        //
        flpLed.AutoSize = true;
        flpLed.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flpLed.Dock = DockStyle.Top;
        flpLed.Location = new Point(8, 260);
        flpLed.Name = "flpLed";
        flpLed.Size = new Size(248, 21);
        flpLed.TabIndex = 10;
        flpLed.WrapContents = false;
        flpLed.Controls.Add(lblLedDot);
        flpLed.Controls.Add(lblLedText);

        lblLedDot.AutoSize = true;
        lblLedDot.Font = new Font("Segoe UI", 12F);
        lblLedDot.ForeColor = Color.Firebrick;
        lblLedDot.Location = new Point(3, 0);
        lblLedDot.Name = "lblLedDot";
        lblLedDot.Size = new Size(15, 21);
        lblLedDot.TabIndex = 0;
        lblLedDot.Text = "●";

        lblLedText.Anchor = AnchorStyles.Left;
        lblLedText.AutoSize = true;
        lblLedText.Location = new Point(24, 3);
        lblLedText.Margin = new Padding(3, 3, 3, 0);
        lblLedText.Name = "lblLedText";
        lblLedText.Size = new Size(79, 15);
        lblLedText.TabIndex = 1;
        lblLedText.Text = "Disconnected";

        //
        // lblConnHint
        //
        lblConnHint.AutoSize = true;
        lblConnHint.Dock = DockStyle.Top;
        lblConnHint.ForeColor = SystemColors.GrayText;
        lblConnHint.Location = new Point(8, 281);
        lblConnHint.MaximumSize = new Size(240, 0);
        lblConnHint.Name = "lblConnHint";
        lblConnHint.Padding = new Padding(0, 4, 0, 0);
        lblConnHint.Size = new Size(0, 19);
        lblConnHint.TabIndex = 11;

        //
        // grpTx
        //
        grpTx.AutoSize = true;
        grpTx.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        grpTx.Dock = DockStyle.Top;
        grpTx.Margin = new Padding(0, 0, 0, 8);
        grpTx.Name = "grpTx";
        grpTx.Padding = new Padding(8);
        grpTx.Size = new Size(264, 120);
        grpTx.TabIndex = 1;
        grpTx.Text = "TX options";
        grpTx.Controls.Add(tblTx);

        tblTx.AutoSize = true;
        tblTx.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        tblTx.ColumnCount = 2;
        tblTx.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75F));
        tblTx.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tblTx.Dock = DockStyle.Top;
        tblTx.Location = new Point(8, 23);
        tblTx.Name = "tblTx";
        tblTx.Padding = new Padding(0, 0, 0, 4);
        tblTx.RowCount = 3;
        tblTx.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblTx.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblTx.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblTx.Size = new Size(248, 89);
        tblTx.TabIndex = 0;
        tblTx.Controls.Add(lblTxMode, 0, 0);
        tblTx.Controls.Add(cmbTxMode, 1, 0);
        tblTx.Controls.Add(lblLineEnding, 0, 1);
        tblTx.Controls.Add(cmbLineEnding, 1, 1);
        tblTx.Controls.Add(lblEncoding, 0, 2);
        tblTx.Controls.Add(cmbEncoding, 1, 2);

        lblTxMode.Anchor = AnchorStyles.Left;
        lblTxMode.AutoSize = true;
        lblTxMode.Location = new Point(3, 9);
        lblTxMode.Margin = new Padding(3, 6, 3, 0);
        lblTxMode.Name = "lblTxMode";
        lblTxMode.Size = new Size(37, 15);
        lblTxMode.TabIndex = 0;
        lblTxMode.Text = "Mode";

        cmbTxMode.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        cmbTxMode.Dock = DockStyle.Fill;
        cmbTxMode.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbTxMode.FormattingEnabled = true;
        cmbTxMode.Items.AddRange(new object[] { "ASCII", "HEX" });
        cmbTxMode.Location = new Point(78, 3);
        cmbTxMode.Margin = new Padding(0, 3, 0, 3);
        cmbTxMode.Name = "cmbTxMode";
        cmbTxMode.Size = new Size(170, 23);
        cmbTxMode.TabIndex = 10;
        _tips.SetToolTip(cmbTxMode, "HEX mode expects byte pairs like “48 65 6C”");
        cmbTxMode.SelectedIndexChanged += CmbTxMode_SelectedIndexChanged;

        lblLineEnding.Anchor = AnchorStyles.Left;
        lblLineEnding.AutoSize = true;
        lblLineEnding.Location = new Point(3, 38);
        lblLineEnding.Margin = new Padding(3, 6, 3, 0);
        lblLineEnding.Name = "lblLineEnding";
        lblLineEnding.Size = new Size(55, 15);
        lblLineEnding.TabIndex = 0;
        lblLineEnding.Text = "Line end";

        cmbLineEnding.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        cmbLineEnding.Dock = DockStyle.Fill;
        cmbLineEnding.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbLineEnding.FormattingEnabled = true;
        cmbLineEnding.Items.AddRange(new object[] { "None", "LF", "CR", "CRLF" });
        cmbLineEnding.Location = new Point(78, 32);
        cmbLineEnding.Margin = new Padding(0, 3, 0, 3);
        cmbLineEnding.Name = "cmbLineEnding";
        cmbLineEnding.Size = new Size(170, 23);
        cmbLineEnding.TabIndex = 11;

        lblEncoding.Anchor = AnchorStyles.Left;
        lblEncoding.AutoSize = true;
        lblEncoding.Location = new Point(3, 67);
        lblEncoding.Margin = new Padding(3, 6, 3, 0);
        lblEncoding.Name = "lblEncoding";
        lblEncoding.Size = new Size(58, 15);
        lblEncoding.TabIndex = 0;
        lblEncoding.Text = "Encoding";

        cmbEncoding.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        cmbEncoding.Dock = DockStyle.Fill;
        cmbEncoding.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbEncoding.FormattingEnabled = true;
        cmbEncoding.Items.AddRange(new object[] { "ASCII", "UTF8", "Latin1" });
        cmbEncoding.Location = new Point(78, 61);
        cmbEncoding.Margin = new Padding(0, 3, 0, 3);
        cmbEncoding.Name = "cmbEncoding";
        cmbEncoding.Size = new Size(170, 23);
        cmbEncoding.TabIndex = 12;

        //
        // grpView
        //
        grpView.AutoSize = true;
        grpView.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        grpView.Dock = DockStyle.Top;
        grpView.Margin = new Padding(0, 0, 0, 8);
        grpView.Name = "grpView";
        grpView.Padding = new Padding(8);
        grpView.Size = new Size(264, 330);
        grpView.TabIndex = 2;
        grpView.Text = "View / log";
        // Bottom-most first.
        grpView.Controls.Add(flpSettingsButtons);
        grpView.Controls.Add(tblLogPath);
        grpView.Controls.Add(flpViewChecks);
        grpView.Controls.Add(tblView);

        tblView.AutoSize = true;
        tblView.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        tblView.ColumnCount = 2;
        tblView.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75F));
        tblView.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tblView.Dock = DockStyle.Top;
        tblView.Location = new Point(8, 23);
        tblView.Name = "tblView";
        tblView.Padding = new Padding(0, 0, 0, 4);
        tblView.RowCount = 1;
        tblView.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblView.Size = new Size(248, 33);
        tblView.TabIndex = 0;
        tblView.Controls.Add(lblDisplay, 0, 0);
        tblView.Controls.Add(cmbDisplay, 1, 0);

        lblDisplay.Anchor = AnchorStyles.Left;
        lblDisplay.AutoSize = true;
        lblDisplay.Location = new Point(3, 9);
        lblDisplay.Margin = new Padding(3, 6, 3, 0);
        lblDisplay.Name = "lblDisplay";
        lblDisplay.Size = new Size(47, 15);
        lblDisplay.TabIndex = 0;
        lblDisplay.Text = "Display";

        cmbDisplay.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        cmbDisplay.Dock = DockStyle.Fill;
        cmbDisplay.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbDisplay.FormattingEnabled = true;
        cmbDisplay.Items.AddRange(new object[] { "ASCII", "HEX" });
        cmbDisplay.Location = new Point(78, 3);
        cmbDisplay.Margin = new Padding(0, 3, 0, 3);
        cmbDisplay.Name = "cmbDisplay";
        cmbDisplay.Size = new Size(170, 23);
        cmbDisplay.TabIndex = 13;
        _tips.SetToolTip(cmbDisplay, "How received bytes are rendered");
        cmbDisplay.SelectedIndexChanged += CmbDisplay_SelectedIndexChanged;

        flpViewChecks.AutoSize = true;
        flpViewChecks.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flpViewChecks.Dock = DockStyle.Top;
        flpViewChecks.FlowDirection = FlowDirection.TopDown;
        flpViewChecks.Location = new Point(8, 56);
        flpViewChecks.Name = "flpViewChecks";
        flpViewChecks.Size = new Size(248, 175);
        flpViewChecks.TabIndex = 1;
        flpViewChecks.WrapContents = false;
        flpViewChecks.Controls.Add(chkEcho);
        flpViewChecks.Controls.Add(chkTimestamp);
        flpViewChecks.Controls.Add(chkAutoscroll);
        flpViewChecks.Controls.Add(chkWrap);
        flpViewChecks.Controls.Add(chkDarkTerminal);
        flpViewChecks.Controls.Add(chkColorize);
        flpViewChecks.Controls.Add(chkLogging);

        chkEcho.AutoSize = true;
        chkEcho.Checked = true;
        chkEcho.CheckState = CheckState.Checked;
        chkEcho.Location = new Point(3, 3);
        chkEcho.Name = "chkEcho";
        chkEcho.Size = new Size(83, 19);
        chkEcho.TabIndex = 14;
        chkEcho.Text = "Local echo";

        chkTimestamp.AutoSize = true;
        chkTimestamp.Location = new Point(3, 28);
        chkTimestamp.Name = "chkTimestamp";
        chkTimestamp.Size = new Size(84, 19);
        chkTimestamp.TabIndex = 15;
        chkTimestamp.Text = "Timestamp";
        chkTimestamp.CheckedChanged += ChkTimestamp_CheckedChanged;

        chkAutoscroll.AutoSize = true;
        chkAutoscroll.Checked = true;
        chkAutoscroll.CheckState = CheckState.Checked;
        chkAutoscroll.Location = new Point(3, 53);
        chkAutoscroll.Name = "chkAutoscroll";
        chkAutoscroll.Size = new Size(88, 19);
        chkAutoscroll.TabIndex = 16;
        chkAutoscroll.Text = "Auto-scroll";
        chkAutoscroll.CheckedChanged += ChkAutoscroll_CheckedChanged;

        chkWrap.AutoSize = true;
        chkWrap.Checked = true;
        chkWrap.CheckState = CheckState.Checked;
        chkWrap.Location = new Point(3, 78);
        chkWrap.Name = "chkWrap";
        chkWrap.Size = new Size(84, 19);
        chkWrap.TabIndex = 17;
        chkWrap.Text = "Word wrap";
        chkWrap.CheckedChanged += ChkWrap_CheckedChanged;

        chkDarkTerminal.AutoSize = true;
        chkDarkTerminal.Location = new Point(3, 103);
        chkDarkTerminal.Name = "chkDarkTerminal";
        chkDarkTerminal.Size = new Size(97, 19);
        chkDarkTerminal.TabIndex = 18;
        chkDarkTerminal.Text = "Dark terminal";
        chkDarkTerminal.CheckedChanged += ChkDarkTerminal_CheckedChanged;

        chkColorize.AutoSize = true;
        chkColorize.Checked = true;
        chkColorize.CheckState = CheckState.Checked;
        chkColorize.Location = new Point(3, 128);
        chkColorize.Name = "chkColorize";
        chkColorize.Size = new Size(106, 19);
        chkColorize.TabIndex = 19;
        chkColorize.Text = "Colorize RX/TX";
        chkColorize.CheckedChanged += ChkColorize_CheckedChanged;

        chkLogging.AutoSize = true;
        chkLogging.Location = new Point(3, 153);
        chkLogging.Name = "chkLogging";
        chkLogging.Size = new Size(110, 19);
        chkLogging.TabIndex = 20;
        chkLogging.Text = "Enable logging";
        chkLogging.CheckedChanged += ChkLogging_CheckedChanged;

        //
        // tblLogPath — TextBox Fill + Browse 75px
        //
        tblLogPath.ColumnCount = 2;
        tblLogPath.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tblLogPath.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75F));
        tblLogPath.Dock = DockStyle.Top;
        tblLogPath.Location = new Point(8, 231);
        tblLogPath.Name = "tblLogPath";
        tblLogPath.RowCount = 1;
        tblLogPath.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblLogPath.Size = new Size(248, 29);
        tblLogPath.TabIndex = 2;
        tblLogPath.Controls.Add(txtLogPath, 0, 0);
        tblLogPath.Controls.Add(btnBrowseLog, 1, 0);

        txtLogPath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        txtLogPath.Dock = DockStyle.Fill;
        txtLogPath.Location = new Point(3, 3);
        txtLogPath.Name = "txtLogPath";
        txtLogPath.PlaceholderText = "./logs/… (blank = default)";
        txtLogPath.Size = new Size(167, 23);
        txtLogPath.TabIndex = 21;
        _tips.SetToolTip(txtLogPath, "Log file path — blank uses ./logs/SerialCOM_<time>.log");

        btnBrowseLog.Anchor = AnchorStyles.Right;
        btnBrowseLog.Location = new Point(173, 3);
        btnBrowseLog.Name = "btnBrowseLog";
        btnBrowseLog.Size = new Size(75, 23);
        btnBrowseLog.TabIndex = 22;
        btnBrowseLog.Text = "Browse…";
        btnBrowseLog.Click += BtnBrowseLog_Click;

        flpSettingsButtons.AutoSize = true;
        flpSettingsButtons.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flpSettingsButtons.Dock = DockStyle.Top;
        flpSettingsButtons.Location = new Point(8, 260);
        flpSettingsButtons.Name = "flpSettingsButtons";
        flpSettingsButtons.Size = new Size(248, 29);
        flpSettingsButtons.TabIndex = 3;
        flpSettingsButtons.WrapContents = true;
        flpSettingsButtons.Controls.Add(btnSaveSettings);
        flpSettingsButtons.Controls.Add(btnLoadSettings);
        flpSettingsButtons.Controls.Add(btnDefaultSettings);

        btnSaveSettings.AutoSize = true;
        btnSaveSettings.Location = new Point(3, 3);
        btnSaveSettings.Name = "btnSaveSettings";
        btnSaveSettings.Size = new Size(46, 23);
        btnSaveSettings.TabIndex = 23;
        btnSaveSettings.Text = "Save";
        _tips.SetToolTip(btnSaveSettings, "Save all settings now");
        btnSaveSettings.Click += BtnSaveSettings_Click;

        btnLoadSettings.AutoSize = true;
        btnLoadSettings.Location = new Point(55, 3);
        btnLoadSettings.Name = "btnLoadSettings";
        btnLoadSettings.Size = new Size(46, 23);
        btnLoadSettings.TabIndex = 24;
        btnLoadSettings.Text = "Load";
        _tips.SetToolTip(btnLoadSettings, "Reload saved settings from disk");
        btnLoadSettings.Click += BtnLoadSettings_Click;

        btnDefaultSettings.AutoSize = true;
        btnDefaultSettings.Location = new Point(107, 3);
        btnDefaultSettings.Name = "btnDefaultSettings";
        btnDefaultSettings.Size = new Size(56, 23);
        btnDefaultSettings.TabIndex = 25;
        btnDefaultSettings.Text = "Default";
        _tips.SetToolTip(btnDefaultSettings, "Restore factory defaults");
        btnDefaultSettings.Click += BtnDefaultSettings_Click;

        //
        // split.Panel2 — right side
        //
        split.Panel1.Controls.Add(leftPanel);

        optsBar.AutoSize = true;
        optsBar.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        optsBar.Dock = DockStyle.Top;
        optsBar.Location = new Point(0, 0);
        optsBar.Name = "optsBar";
        optsBar.Padding = new Padding(4);
        optsBar.Size = new Size(716, 33);
        optsBar.TabIndex = 0;
        optsBar.WrapContents = true;
        optsBar.Controls.Add(chkAutoReconnect);
        optsBar.Controls.Add(lblHint);

        chkAutoReconnect.AutoSize = true;
        chkAutoReconnect.Location = new Point(7, 7);
        chkAutoReconnect.Name = "chkAutoReconnect";
        chkAutoReconnect.Size = new Size(112, 19);
        chkAutoReconnect.TabIndex = 26;
        chkAutoReconnect.Text = "Auto-reconnect";
        _tips.SetToolTip(chkAutoReconnect, "Retry the port every 2 s after an unexpected drop");

        lblHint.AutoSize = true;
        lblHint.ForeColor = SystemColors.GrayText;
        lblHint.Location = new Point(125, 7);
        lblHint.Margin = new Padding(3, 6, 3, 0);
        lblHint.Name = "lblHint";
        lblHint.Size = new Size(208, 15);
        lblHint.TabIndex = 27;
        lblHint.Text = "F5 refresh · Ctrl+L clear · ↑/↓ history";

        sendBar.ColumnCount = 3;
        sendBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        sendBar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75F));
        sendBar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        sendBar.Dock = DockStyle.Bottom;
        sendBar.Height = 38;
        sendBar.Location = new Point(0, 541);
        sendBar.Name = "sendBar";
        sendBar.Padding = new Padding(4);
        sendBar.RowCount = 1;
        sendBar.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        sendBar.Size = new Size(716, 38);
        sendBar.TabIndex = 1;
        sendBar.Controls.Add(txtSend, 0, 0);
        sendBar.Controls.Add(btnSend, 1, 0);
        sendBar.Controls.Add(btnSendFile, 2, 0);

        txtSend.Dock = DockStyle.Fill;
        txtSend.Location = new Point(7, 7);
        txtSend.Name = "txtSend";
        txtSend.PlaceholderText = "Type to send…  (Enter = Send · ↑/↓ history)";
        txtSend.Size = new Size(513, 23);
        txtSend.TabIndex = 28;
        _tips.SetToolTip(txtSend, "Enter sends · Up/Down recalls history · HEX mode needs pairs like “48 65”");
        txtSend.KeyDown += TxtSend_KeyDown;
        txtSend.TextChanged += TxtSend_TextChanged;

        btnSend.Anchor = AnchorStyles.Right;
        btnSend.Location = new Point(527, 7);
        btnSend.Name = "btnSend";
        btnSend.Size = new Size(75, 23);
        btnSend.TabIndex = 29;
        btnSend.Text = "Send";
        btnSend.Click += BtnSend_Click;

        btnSendFile.Anchor = AnchorStyles.Right;
        btnSendFile.AutoSize = true;
        btnSendFile.Location = new Point(608, 6);
        btnSendFile.Name = "btnSendFile";
        btnSendFile.Size = new Size(101, 25);
        btnSendFile.TabIndex = 30;
        btnSendFile.Text = "Send File…";
        _tips.SetToolTip(btnSendFile, "Transmit a file as raw bytes");
        btnSendFile.Click += BtnSendFile_Click;

        rtbTerminal.BackColor = SystemColors.Window;
        rtbTerminal.Dock = DockStyle.Fill;
        rtbTerminal.DetectUrls = false;
        rtbTerminal.Font = new Font("Consolas", 10F);
        rtbTerminal.ForeColor = SystemColors.WindowText;
        rtbTerminal.HideSelection = false;
        rtbTerminal.Location = new Point(0, 33);
        rtbTerminal.Name = "rtbTerminal";
        rtbTerminal.ReadOnly = true;
        rtbTerminal.Size = new Size(716, 508);
        rtbTerminal.TabIndex = 31;
        rtbTerminal.Text = "";
        rtbTerminal.WordWrap = true;

        // Fill first, then Bottom, then Top — so Fill respects the bars.
        split.Panel2.Controls.Add(rtbTerminal);
        split.Panel2.Controls.Add(sendBar);
        split.Panel2.Controls.Add(optsBar);

        //
        // MainForm (top-level: Fill, Bottom, Top…)
        //
        Controls.Add(split);
        Controls.Add(statusStrip);
        Controls.Add(toolStrip);
        Controls.Add(menuStrip);
        MainMenuStrip = menuStrip;

        KeyDown += MainForm_KeyDown;
        FormClosing += MainForm_FormClosing;

        menuStrip.ResumeLayout(false);
        menuStrip.PerformLayout();
        toolStrip.ResumeLayout(false);
        toolStrip.PerformLayout();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)split).EndInit();
        split.ResumeLayout(false);
        split.Panel1.ResumeLayout(false);
        split.Panel2.ResumeLayout(false);
        leftPanel.ResumeLayout(false);
        leftPanel.PerformLayout();
        grpConnection.ResumeLayout(false);
        grpConnection.PerformLayout();
        grpTx.ResumeLayout(false);
        grpTx.PerformLayout();
        grpView.ResumeLayout(false);
        grpView.PerformLayout();
        tblConnection.ResumeLayout(false);
        tblConnection.PerformLayout();
        tblPort.ResumeLayout(false);
        tblPort.PerformLayout();
        flpDtrRts.ResumeLayout(false);
        flpDtrRts.PerformLayout();
        flpLed.ResumeLayout(false);
        flpLed.PerformLayout();
        tblTx.ResumeLayout(false);
        tblTx.PerformLayout();
        tblView.ResumeLayout(false);
        tblView.PerformLayout();
        flpViewChecks.ResumeLayout(false);
        flpViewChecks.PerformLayout();
        tblLogPath.ResumeLayout(false);
        tblLogPath.PerformLayout();
        flpSettingsButtons.ResumeLayout(false);
        flpSettingsButtons.PerformLayout();
        optsBar.ResumeLayout(false);
        optsBar.PerformLayout();
        sendBar.ResumeLayout(false);
        sendBar.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}
