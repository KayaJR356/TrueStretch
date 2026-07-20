using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

[assembly: System.Reflection.AssemblyTitle("TrueStretch")]
[assembly: System.Reflection.AssemblyDescription("Guvenli ekran modu ve oyun cozunurlugu onerileri")]
[assembly: System.Reflection.AssemblyCompany("Kaya")]
[assembly: System.Reflection.AssemblyProduct("TrueStretch")]
[assembly: System.Reflection.AssemblyVersion("3.0.0.0")]
[assembly: System.Reflection.AssemblyFileVersion("3.0.0.0")]

namespace TrueStretch
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    public sealed class MainForm : Form
    {
        readonly Panel contentHost = new Panel();
        readonly List<Panel> pages = new List<Panel>();
        readonly ComboBox availableModes = new ComboBox();
        readonly NumberInput widthBox = NumberBox(640, 7680, 1568);
        readonly NumberInput heightBox = NumberBox(480, 4320, 1080);
        readonly NumberInput hzBox = NumberBox(24, 360, 60);
        readonly ListBox profiles = new ListBox();
        readonly Label currentMode = new Label();
        readonly Label status = new Label();
        readonly ComboBox games = new ComboBox();
        readonly ListView recommendations = new ListView();
        readonly Label webStatus = new Label();
        readonly CheckBox restartMonitor = new CheckBox();
        readonly List<Button> navButtons = new List<Button>();
        readonly Dictionary<string, string> recommendationLinks = new Dictionary<string, string>();
        readonly string profilePath;
        List<Mode> savedProfiles = new List<Mode>();
        Mode originalMode;

        public MainForm()
        {
            Text = "TrueStretch 3.0";
            ClientSize = new Size(1120, 700);
            MinimumSize = new Size(980, 640);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10F);
            BackColor = Color.FromArgb(9, 13, 24);
            ForeColor = Color.WhiteSmoke;
            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }
            profilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TrueStretch", "profiles.xml");
            originalMode = DisplayApi.Current();
            hzBox.Value = originalMode.Hz;
            BuildUi();
            LoadProfiles();
            RefreshModes();
            FormClosing += delegate { string ignored; DisplayApi.EnablePrimaryMonitor(out ignored); };
        }

        static NumberInput NumberBox(int min, int max, int value)
        {
            return new NumberInput(min, max, value) { Size = new Size(120, 34) };
        }

        void BuildUi()
        {
            var sidebar = new Panel { Dock = DockStyle.Left, Width = 230, BackColor = Color.FromArgb(13, 19, 34) };
            var mark = new Label { Text = "TS", Font = new Font("Segoe UI Black", 17F), TextAlign = ContentAlignment.MiddleCenter, Location = new Point(22, 24), Size = new Size(48, 48), BackColor = Color.FromArgb(100, 92, 255), ForeColor = Color.White };
            var brand = new Label { Text = "TrueStretch", Font = new Font("Segoe UI Semibold", 16F), AutoSize = true, Location = new Point(80, 27), ForeColor = Color.White };
            var version = new Label { Text = "DISPLAY LAB  /  v3.0", Font = new Font("Segoe UI Semibold", 8F), AutoSize = true, Location = new Point(82, 56), ForeColor = Color.FromArgb(112, 126, 155) };
            sidebar.Controls.AddRange(new Control[] { mark, brand, version });
            sidebar.Controls.Add(Nav("EKRAN MODLARI", 112, 0));
            sidebar.Controls.Add(Nav("OZEL PROFILLER", 164, 1));
            sidebar.Controls.Add(Nav("OYUN ONERILERI", 216, 2));
            var safety = new Panel { Location = new Point(18, 530), Size = new Size(194, 125), BackColor = Color.FromArgb(19, 28, 48), Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            safety.Controls.Add(new Label { Text = "GUVENLI MOD", Font = new Font("Segoe UI Semibold", 9F), ForeColor = Color.FromArgb(72, 222, 171), Location = new Point(14, 14), AutoSize = true });
            safety.Controls.Add(new Label { Text = "Her degisiklik test edilir\nve 15 saniyede otomatik\ngeri alinabilir.", ForeColor = Color.FromArgb(164, 177, 202), Location = new Point(14, 43), Size = new Size(170, 66) });
            sidebar.Controls.Add(safety);

            var header = new Panel { Location = new Point(230, 0), Height = 92, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, Width = 890, BackColor = Color.FromArgb(9, 13, 24) };
            var title = new Label { Text = "Ekran deneyimini yonet", Font = new Font("Segoe UI Semibold", 22F), AutoSize = true, Location = new Point(28, 18), ForeColor = Color.White };
            currentMode.AutoSize = true; currentMode.Location = new Point(31, 58); currentMode.ForeColor = Color.FromArgb(137, 151, 181);
            header.Controls.AddRange(new Control[] { title, currentMode });

            contentHost.Location = new Point(230, 92); contentHost.Size = new Size(890, 552); contentHost.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right; contentHost.BackColor = Color.FromArgb(9, 13, 24);
            pages.Add(BuildModesPage());
            pages.Add(BuildProfilesPage());
            pages.Add(BuildRecommendationsPage());
            foreach (Panel page in pages) { page.Dock = DockStyle.Fill; page.Visible = false; contentHost.Controls.Add(page); }
            status.Location = new Point(260, 660); status.AutoSize = true; status.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; status.ForeColor = Color.FromArgb(72, 222, 171);
            Controls.AddRange(new Control[] { sidebar, header, contentHost, status });
            SelectNav(0);
        }

        Button Nav(string text, int y, int index)
        {
            var b = new Button { Text = "   " + text, TextAlign = ContentAlignment.MiddleLeft, Location = new Point(12, y), Size = new Size(206, 44), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Semibold", 9F), BackColor = Color.FromArgb(13, 19, 34), ForeColor = Color.FromArgb(151, 164, 190), Cursor = Cursors.Hand, Tag = index };
            b.FlatAppearance.BorderSize = 0; b.Click += delegate { SelectNav((int)b.Tag); }; navButtons.Add(b); return b;
        }

        void SelectNav(int index)
        {
            if (index < 0 || index >= pages.Count) return;
            for (int i = 0; i < pages.Count; i++) pages[i].Visible = i == index;
            pages[index].BringToFront();
            for (int i = 0; i < navButtons.Count; i++) { bool active = i == index; navButtons[i].BackColor = active ? Color.FromArgb(34, 42, 68) : Color.FromArgb(13, 19, 34); navButtons[i].ForeColor = active ? Color.FromArgb(133, 125, 255) : Color.FromArgb(151, 164, 190); }
        }

        Panel NewPage()
        {
            return new Panel { BackColor = Color.FromArgb(9, 13, 24), ForeColor = Color.WhiteSmoke, Padding = new Padding(18) };
        }

        Panel BuildModesPage()
        {
            var page = NewPage();
            var heading = new Label { Text = "Desteklenen ekran modlari", Font = new Font("Segoe UI Semibold", 18F), AutoSize = true, Location = new Point(24, 18), ForeColor = Color.White };
            var intro = L("Surucunun sundugu bir modu secin. Uygulama once uyumlulugu kontrol eder.", 26, 58, 700);
            var card = new Panel { Location = new Point(24, 112), Size = new Size(790, 190), BackColor = Color.FromArgb(18, 25, 43), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            card.Controls.Add(new Label { Text = "COZUNURLUK VE YENILEME HIZI", Font = new Font("Segoe UI Semibold", 9F), ForeColor = Color.FromArgb(125, 139, 168), Location = new Point(24, 24), AutoSize = true });
            availableModes.DropDownStyle = ComboBoxStyle.DropDownList; availableModes.Location = new Point(24, 58); availableModes.Width = 390; availableModes.Height = 34; availableModes.FlatStyle = FlatStyle.Flat; availableModes.BackColor = Color.FromArgb(27, 35, 57); availableModes.ForeColor = Color.White;
            var refresh = B("Yenile", 435, 55, delegate { RefreshModes(); });
            var apply = B("SECILI MODU UYGULA", 24, 118, delegate { if (availableModes.SelectedItem != null) ApplySafely((Mode)availableModes.SelectedItem); }); apply.Width = 220;
            var restore = B("Baslangic moduna don", 260, 118, delegate { RestoreOriginal(); }); restore.Width = 190; restore.BackColor = Color.FromArgb(38, 47, 70);
            restartMonitor.Text = "TrueStretch aktifken monitor aygitini devre disi tut"; restartMonitor.Checked = true; restartMonitor.AutoSize = true; restartMonitor.Location = new Point(475, 129); restartMonitor.ForeColor = Color.FromArgb(151, 164, 190); restartMonitor.FlatStyle = FlatStyle.Flat;
            card.Controls.AddRange(new Control[] { availableModes, refresh, apply, restore, restartMonitor });
            var tip = new Panel { Location = new Point(24, 322), Size = new Size(790, 94), BackColor = Color.FromArgb(18, 25, 43), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            tip.Controls.Add(new Label { Text = "15", Font = new Font("Segoe UI Black", 22F), ForeColor = Color.FromArgb(72, 222, 171), Location = new Point(22, 20), AutoSize = true });
            tip.Controls.Add(new Label { Text = "SANIYELIK GUVENLIK", Font = new Font("Segoe UI Semibold", 9F), ForeColor = Color.White, Location = new Point(78, 19), AutoSize = true });
            tip.Controls.Add(new Label { Text = "Yeni goruntuyu onaylamazsaniz onceki ayar otomatik geri gelir.", ForeColor = Color.FromArgb(145, 159, 188), Location = new Point(78, 43), AutoSize = true });
            page.Controls.AddRange(new Control[] { heading, intro, card, tip });
            return page;
        }

        Panel BuildProfilesPage()
        {
            var page = NewPage();
            page.Controls.Add(new Label { Text = "Kendi profilini olustur", Font = new Font("Segoe UI Semibold", 18F), AutoSize = true, Location = new Point(24, 18), ForeColor = Color.White });
            page.Controls.Add(L("Genislik", 24, 66, 120)); page.Controls.Add(L("Yukseklik", 174, 66, 120)); page.Controls.Add(L("Yenileme (Hz)", 324, 66, 140));
            widthBox.Location = new Point(24, 96); heightBox.Location = new Point(174, 96); hzBox.Location = new Point(324, 96);
            var add = B("PROFILI KAYDET", 474, 92, delegate { AddProfile(); }); add.Width = 170;
            profiles.Location = new Point(24, 160); profiles.Size = new Size(430, 270); profiles.BackColor = Color.FromArgb(18, 25, 43); profiles.ForeColor = Color.WhiteSmoke; profiles.BorderStyle = BorderStyle.None;
            var apply = B("SECILI PROFILI UYGULA", 480, 160, delegate { if (profiles.SelectedIndex >= 0) ApplySafely(savedProfiles[profiles.SelectedIndex]); }); apply.Width = 210;
            var remove = B("Profili sil", 480, 212, delegate { RemoveProfile(); }); remove.Width = 160; remove.BackColor = Color.FromArgb(38, 47, 70);
            var note = L("Surucu bu modu desteklemiyorsa uygulama islemi guvenli bicimde reddeder. Donanim zamanlamasi zorla degistirilmez.", 480, 290, 320);
            note.ForeColor = Color.Gold;
            page.Controls.AddRange(new Control[] { widthBox, heightBox, hzBox, add, profiles, apply, remove, note });
            return page;
        }

        Panel BuildRecommendationsPage()
        {
            var page = NewPage();
            games.Items.AddRange(new object[] { "Valorant", "Counter-Strike 2", "Fortnite", "Apex Legends", "PUBG", "Rainbow Six Siege", "Overwatch 2", "The Finals" });
            page.Controls.Add(new Label { Text = "Oyunun icin dogru stretch'i bul", Font = new Font("Segoe UI Semibold", 18F), AutoSize = true, Location = new Point(24, 18), ForeColor = Color.White });
            games.SelectedIndex = 0; games.DropDownStyle = ComboBoxStyle.DropDownList; games.Location = new Point(24, 68); games.Width = 270; games.FlatStyle = FlatStyle.Flat; games.BackColor = Color.FromArgb(27, 35, 57); games.ForeColor = Color.White;
            var search = B("WEB'DEN ONER", 314, 64, async delegate { await SearchRecommendations(); }); search.Width = 170;
            webStatus.Location = new Point(510, 72); webStatus.AutoSize = true; webStatus.ForeColor = Color.FromArgb(137, 151, 181);
            recommendations.Location = new Point(24, 125); recommendations.Size = new Size(805, 285); recommendations.View = View.Details; recommendations.FullRowSelect = true; recommendations.BackColor = Color.FromArgb(18, 25, 43); recommendations.ForeColor = Color.WhiteSmoke; recommendations.BorderStyle = BorderStyle.None;
            recommendations.Columns.Add("Cozunurluk", 150); recommendations.Columns.Add("En-boy", 100); recommendations.Columns.Add("Web sonucu", 110); recommendations.Columns.Add("Kaynak / aciklama", 420);
            recommendations.DoubleClick += delegate { OpenSelectedSource(); };
            var use = B("PROFIL OLARAK KAYDET", 24, 430, delegate { SaveSelectedRecommendation(); }); use.Width = 220;
            var source = B("Kaynagi ac", 264, 430, delegate { OpenSelectedSource(); }); source.Width = 145; source.BackColor = Color.FromArgb(38, 47, 70);
            page.Controls.AddRange(new Control[] { games, search, webStatus, recommendations, use, source });
            return page;
        }

        Label L(string text, int x, int y, int width)
        {
            return new Label { Text = text, Location = new Point(x, y), AutoSize = false, Size = new Size(width, 45), ForeColor = Color.FromArgb(151, 164, 190) };
        }

        Button B(string text, int x, int y, EventHandler click)
        {
            var b = new Button { Text = text, Location = new Point(x, y), Size = new Size(135, 40), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(100, 92, 255), ForeColor = Color.White, Font = new Font("Segoe UI Semibold", 9F), Cursor = Cursors.Hand };
            b.FlatAppearance.BorderSize = 0; b.Click += click; return b;
        }

        void RefreshModes()
        {
            availableModes.Items.Clear();
            foreach (var mode in DisplayApi.Enumerate()) availableModes.Items.Add(mode);
            Mode now = DisplayApi.Current(); currentMode.Text = "Mevcut: " + now;
            for (int i = 0; i < availableModes.Items.Count; i++) if (((Mode)availableModes.Items[i]).SameAs(now)) { availableModes.SelectedIndex = i; break; }
            status.Text = availableModes.Items.Count + " desteklenen ekran modu bulundu.";
        }

        void AddProfile()
        {
            var m = new Mode((int)widthBox.Value, (int)heightBox.Value, (int)hzBox.Value, 32);
            foreach (var p in savedProfiles) if (p.SameAs(m)) { status.Text = "Bu profil zaten kayitli."; return; }
            savedProfiles.Add(m); SaveProfiles(); RenderProfiles(); status.Text = "Profil kaydedildi: " + m;
        }

        void RemoveProfile()
        {
            if (profiles.SelectedIndex < 0) return;
            savedProfiles.RemoveAt(profiles.SelectedIndex); SaveProfiles(); RenderProfiles(); status.Text = "Profil silindi.";
        }

        void LoadProfiles()
        {
            try
            {
                if (File.Exists(profilePath))
                {
                    var doc = new XmlDocument(); doc.Load(profilePath);
                    foreach (XmlNode n in doc.SelectNodes("/profiles/mode")) savedProfiles.Add(new Mode(Int(n, "width"), Int(n, "height"), Int(n, "hz"), 32));
                }
            }
            catch { savedProfiles = new List<Mode>(); }
            RenderProfiles();
        }

        int Int(XmlNode n, string a) { return int.Parse(n.Attributes[a].Value, CultureInfo.InvariantCulture); }

        void SaveProfiles()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(profilePath));
            var settings = new XmlWriterSettings { Indent = true, Encoding = new UTF8Encoding(false) };
            using (var w = XmlWriter.Create(profilePath, settings))
            {
                w.WriteStartElement("profiles");
                foreach (var m in savedProfiles) { w.WriteStartElement("mode"); w.WriteAttributeString("width", m.Width.ToString()); w.WriteAttributeString("height", m.Height.ToString()); w.WriteAttributeString("hz", m.Hz.ToString()); w.WriteEndElement(); }
                w.WriteEndElement();
            }
        }

        void RenderProfiles() { profiles.Items.Clear(); foreach (var p in savedProfiles) profiles.Items.Add(p.ToString()); }

        void ApplySafely(Mode target)
        {
            Mode before = DisplayApi.Current();
            string error;
            Mode requested = target;
            target = DisplayApi.ResolveMode(target);
            if (!target.SameAs(requested)) status.Text = "Istenen Hz bulunamadi; en yakin desteklenen mod kullaniliyor: " + target;
            if (!DisplayApi.Test(target, true, out error)) { MessageBox.Show(error, "Mod desteklenmiyor", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (!DisplayApi.Apply(target, true, out error)) { MessageBox.Show(error, "Uygulanamadi", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            if (!string.IsNullOrEmpty(error)) MessageBox.Show(error, "Olcekleme uyarisi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            if (restartMonitor.Checked && !DisplayApi.DisablePrimaryMonitor(out error)) MessageBox.Show("Cozunurluk stretch olarak uygulandi ancak monitor aygiti devre disi birakilamadi.\n\n" + error, "Monitor aygiti", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            var result = TimedConfirm.Show(this, target.ToString(), 15);
            if (result != DialogResult.Yes) { DisplayApi.Apply(before, false, out error); DisplayApi.EnablePrimaryMonitor(out error); status.Text = "Degisiklik otomatik geri alindi ve monitor yeniden etkinlestirildi."; }
            else { status.Text = "TrueStretch aktif: " + target + " / tam ekran olcekleme"; }
            RefreshModes();
        }

        void RestoreOriginal()
        {
            string error;
            bool modeOk = DisplayApi.Apply(originalMode, false, out error);
            string modeError = error;
            bool monitorOk = DisplayApi.EnablePrimaryMonitor(out error);
            if (!modeOk || !monitorOk) MessageBox.Show((!modeOk ? modeError + "\n" : "") + (!monitorOk ? error : ""), "Normal moda donus", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            status.Text = modeOk && monitorOk ? "Normal mod geri getirildi ve monitor etkinlestirildi." : "Normal moda donus kismen tamamlandi.";
            RefreshModes();
        }

        async Task SearchRecommendations()
        {
            string game = games.SelectedItem.ToString(); webStatus.Text = "Web araniyor..."; recommendations.Items.Clear(); recommendationLinks.Clear();
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string query = Uri.EscapeDataString(game + " best stretched resolution competitive");
                string url = "https://www.bing.com/search?format=rss&q=" + query;
                string xml;
                using (var wc = new WebClient()) { wc.Headers[HttpRequestHeader.UserAgent] = "TrueStretch/3.0"; xml = await wc.DownloadStringTaskAsync(url); }
                var doc = new XmlDocument(); doc.LoadXml(xml);
                var counts = new Dictionary<string, int>(); var details = new Dictionary<string, string>(); var links = new Dictionary<string, string>();
                foreach (XmlNode item in doc.SelectNodes("//item"))
                {
                    string text = (item["title"] == null ? "" : item["title"].InnerText) + " " + (item["description"] == null ? "" : item["description"].InnerText);
                    string link = item["link"] == null ? "" : item["link"].InnerText;
                    foreach (Match m in Regex.Matches(text, @"(?<!\d)(\d{3,4})\s*[xX×]\s*(\d{3,4})(?!\d)"))
                    {
                        int w = int.Parse(m.Groups[1].Value), h = int.Parse(m.Groups[2].Value); if (w < 800 || h < 600 || w > 7680 || h > 4320) continue;
                        string key = w + "x" + h; counts[key] = counts.ContainsKey(key) ? counts[key] + 1 : 1; if (!details.ContainsKey(key)) { details[key] = Strip(item["title"] == null ? text : item["title"].InnerText); links[key] = link; }
                    }
                }
                var keys = new List<string>(counts.Keys); keys.Sort(delegate(string a, string b) { return counts[b].CompareTo(counts[a]); });
                foreach (string key in keys) AddRecommendation(key, counts[key], details[key], links[key]);
                if (recommendations.Items.Count == 0) AddFallback(game);
                webStatus.Text = recommendations.Items.Count + " oneri bulundu. Cift tik: kaynak.";
            }
            catch (Exception ex) { AddFallback(game); webStatus.Text = "Web erisilemedi; yerel oneriler gosteriliyor. " + ex.Message; }
        }

        string Strip(string value) { return Regex.Replace(WebUtility.HtmlDecode(Regex.Replace(value, "<.*?>", " ")), @"\s+", " ").Trim(); }

        void AddRecommendation(string key, int count, string detail, string link)
        {
            string[] p = key.Split('x'); int w = int.Parse(p[0]), h = int.Parse(p[1]); int gcd = Gcd(w, h); string ratio = (w / gcd) + ":" + (h / gcd);
            var item = new ListViewItem(new[] { key, ratio, count.ToString(), detail }); recommendations.Items.Add(item); recommendationLinks[key] = link;
        }

        int Gcd(int a, int b) { while (b != 0) { int t = a % b; a = b; b = t; } return a; }

        void AddFallback(string game)
        {
            string[] modes = game == "Valorant" ? new[] { "1280x960", "1440x1080", "1568x1080", "1280x1024" } : new[] { "1280x960", "1440x1080", "1728x1080", "1920x1080" };
            foreach (string m in modes) AddRecommendation(m, 0, "Yerel baslangic onerisi; web kaynagiyla dogrulayin.", "https://www.bing.com/search?q=" + Uri.EscapeDataString(game + " " + m + " stretched resolution"));
        }

        void SaveSelectedRecommendation()
        {
            if (recommendations.SelectedItems.Count == 0) return; string[] p = recommendations.SelectedItems[0].Text.Split('x');
            widthBox.Value = int.Parse(p[0]); heightBox.Value = int.Parse(p[1]); hzBox.Value = DisplayApi.Current().Hz; AddProfile(); SelectNav(1);
        }

        void OpenSelectedSource()
        {
            if (recommendations.SelectedItems.Count == 0) return; string key = recommendations.SelectedItems[0].Text; string url;
            if (recommendationLinks.TryGetValue(key, out url) && !string.IsNullOrEmpty(url)) Process.Start(url);
        }
    }

    public sealed class NumberInput : Panel
    {
        readonly TextBox edit = new TextBox();
        readonly int minimum, maximum;

        public NumberInput(int min, int max, int value)
        {
            minimum = min; maximum = max;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Color.FromArgb(23, 31, 52); ForeColor = Color.White;
            edit.BorderStyle = BorderStyle.None; edit.BackColor = BackColor; edit.ForeColor = ForeColor; edit.Font = new Font("Segoe UI Semibold", 11F); edit.TextAlign = HorizontalAlignment.Center; edit.Text = value.ToString();
            edit.KeyPress += delegate(object sender, KeyPressEventArgs e) { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };
            edit.Leave += delegate { Value = Value; };
            Controls.Add(edit);
            Resize += delegate { LayoutChildren(); }; LayoutChildren();
        }

        void LayoutChildren()
        {
            edit.SetBounds(10, Math.Max(5, (Height - edit.PreferredHeight) / 2), Math.Max(20, Width - 20), edit.PreferredHeight);
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) { base.SetBoundsCore(x, y, width, 38, specified | BoundsSpecified.Height); }
        protected override void OnPaint(PaintEventArgs e) { base.OnPaint(e); using (var pen = new Pen(Color.FromArgb(61, 75, 108))) e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1); }

        public decimal Value
        {
            get { int parsed; if (!int.TryParse(edit.Text, out parsed)) parsed = minimum; return Math.Max(minimum, Math.Min(maximum, parsed)); }
            set { int v = (int)value; edit.Text = Math.Max(minimum, Math.Min(maximum, v)).ToString(); }
        }
    }

    public sealed class Mode
    {
        public int Width, Height, Hz, Bits;
        public Mode(int w, int h, int hz, int bits) { Width = w; Height = h; Hz = hz; Bits = bits; }
        public bool SameAs(Mode m) { return Width == m.Width && Height == m.Height && Hz == m.Hz; }
        public override string ToString() { return Width + " x " + Height + "  @ " + Hz + " Hz"; }
    }

    static class DisplayApi
    {
        const int ENUM_CURRENT_SETTINGS = -1, CDS_TEST = 2, CDS_UPDATEREGISTRY = 1, DISP_CHANGE_SUCCESSFUL = 0;
        const uint DM_BITSPERPEL = 0x40000, DM_PELSWIDTH = 0x80000, DM_PELSHEIGHT = 0x100000, DM_DISPLAYFREQUENCY = 0x400000, DM_DISPLAYFIXEDOUTPUT = 0x20000000;
        const int DMDFO_DEFAULT = 0, DMDFO_STRETCH = 2;
        static uint disabledMonitorNode;
        static bool monitorDisabled;
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string dmDeviceName; public short dmSpecVersion, dmDriverVersion, dmSize, dmDriverExtra; public int dmFields;
            public int dmPositionX, dmPositionY, dmDisplayOrientation, dmDisplayFixedOutput; public short dmColor, dmDuplex, dmYResolution, dmTTOption, dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string dmFormName; public short dmLogPixels; public int dmBitsPerPel, dmPelsWidth, dmPelsHeight, dmDisplayFlags, dmDisplayFrequency;
            public int dmICMMethod, dmICMIntent, dmMediaType, dmDitherType, dmReserved1, dmReserved2, dmPanningWidth, dmPanningHeight;
        }
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern int ChangeDisplaySettingsEx(string deviceName, ref DEVMODE devMode, IntPtr hwnd, uint flags, IntPtr param);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct DISPLAY_DEVICE
        {
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string DeviceString;
            public int StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string DeviceKey;
        }
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);
        [DllImport("cfgmgr32.dll", CharSet = CharSet.Unicode)] static extern int CM_Locate_DevNodeW(ref uint pdnDevInst, string pDeviceID, uint ulFlags);
        [DllImport("cfgmgr32.dll")] static extern int CM_Disable_DevNode(uint dnDevInst, uint ulFlags);
        [DllImport("cfgmgr32.dll")] static extern int CM_Enable_DevNode(uint dnDevInst, uint ulFlags);
        static DEVMODE Dev() { var d = new DEVMODE(); d.dmDeviceName = new string('\0', 32); d.dmFormName = new string('\0', 32); d.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE)); return d; }
        public static Mode Current() { var d = Dev(); EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref d); return new Mode(d.dmPelsWidth, d.dmPelsHeight, d.dmDisplayFrequency, d.dmBitsPerPel); }
        public static List<Mode> Enumerate()
        {
            var result = new List<Mode>(); var seen = new HashSet<string>();
            for (int i = 0; ; i++) { var d = Dev(); if (!EnumDisplaySettings(null, i, ref d)) break; var m = new Mode(d.dmPelsWidth, d.dmPelsHeight, d.dmDisplayFrequency, d.dmBitsPerPel); string k = m.Width + "x" + m.Height + "@" + m.Hz; if (seen.Add(k)) result.Add(m); }
            result.Sort(delegate(Mode a, Mode b) { int c = a.Width.CompareTo(b.Width); if (c == 0) c = a.Height.CompareTo(b.Height); if (c == 0) c = a.Hz.CompareTo(b.Hz); return c; }); return result;
        }
        static DEVMODE From(Mode m) { var d = Dev(); d.dmPelsWidth = m.Width; d.dmPelsHeight = m.Height; d.dmDisplayFrequency = m.Hz; d.dmBitsPerPel = m.Bits <= 0 ? 32 : m.Bits; d.dmFields = (int)(DM_PELSWIDTH | DM_PELSHEIGHT | DM_DISPLAYFREQUENCY | DM_BITSPERPEL); return d; }
        public static Mode ResolveMode(Mode requested)
        {
            Mode best = null; int distance = int.MaxValue;
            foreach (Mode candidate in Enumerate()) if (candidate.Width == requested.Width && candidate.Height == requested.Height) { int d = Math.Abs(candidate.Hz - requested.Hz); if (d < distance) { best = candidate; distance = d; } }
            return best ?? requested;
        }
        public static bool Test(Mode m, bool stretch, out string error) { var d = From(m); int r = ChangeDisplaySettingsEx(null, ref d, IntPtr.Zero, CDS_TEST, IntPtr.Zero); error = Explain(r); return r == DISP_CHANGE_SUCCESSFUL; }
        public static bool Apply(Mode m, bool stretch, out string error)
        {
            var d = From(m); int r = ChangeDisplaySettingsEx(null, ref d, IntPtr.Zero, CDS_UPDATEREGISTRY, IntPtr.Zero);
            if (r != DISP_CHANGE_SUCCESSFUL) { error = Explain(r); return false; }
            error = "";
            var scaling = Dev(); EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref scaling); scaling.dmDisplayFixedOutput = stretch ? DMDFO_STRETCH : DMDFO_DEFAULT; scaling.dmFields = (int)DM_DISPLAYFIXEDOUTPUT;
            int scaleResult = ChangeDisplaySettingsEx(null, ref scaling, IntPtr.Zero, CDS_UPDATEREGISTRY, IntPtr.Zero);
            if (stretch && scaleResult != DISP_CHANGE_SUCCESSFUL) error = "Cozunurluk uygulandi fakat ekran karti surucusu Windows tam ekran olcekleme istegini kabul etmedi (kod " + scaleResult + "). GPU kontrol panelinde olcekleme modunu Tam Ekran olarak secin.";
            return true;
        }
        public static bool DisablePrimaryMonitor(out string error)
        {
            error = "";
            if (monitorDisabled) return true;
            uint node;
            if (!TryFindPrimaryMonitorNode(out node, out error)) return false;
            int disable = CM_Disable_DevNode(node, 0);
            if (disable != 0) { error = "Monitor devre disi birakilamadi (CM hata " + disable + ")."; return false; }
            disabledMonitorNode = node; monitorDisabled = true; System.Threading.Thread.Sleep(800); return true;
        }
        public static bool EnablePrimaryMonitor(out string error)
        {
            error = "";
            if (!monitorDisabled) return true;
            int enable = CM_Enable_DevNode(disabledMonitorNode, 0);
            if (enable != 0) { error = "Monitor yeniden etkinlestirilemedi (CM hata " + enable + ")."; return false; }
            monitorDisabled = false; disabledMonitorNode = 0; System.Threading.Thread.Sleep(800); return true;
        }
        static bool TryFindPrimaryMonitorNode(out uint node, out string error)
        {
            error = ""; node = 0; string instanceId = null;
            for (uint i = 0; ; i++)
            {
                var adapter = NewDisplayDevice(); if (!EnumDisplayDevices(null, i, ref adapter, 0)) break;
                if ((adapter.StateFlags & 4) == 0) continue;
                var monitor = NewDisplayDevice();
                if (EnumDisplayDevices(adapter.DeviceName, 0, ref monitor, 1)) instanceId = monitor.DeviceID;
                break;
            }
            if (string.IsNullOrEmpty(instanceId)) { error = "Birincil monitor aygit kimligi bulunamadi."; return false; }
            int locate = CM_Locate_DevNodeW(ref node, instanceId, 0);
            if (locate != 0) { error = "Monitor aygiti bulunamadi (CM hata " + locate + ")."; return false; }
            return true;
        }
        static DISPLAY_DEVICE NewDisplayDevice() { var d = new DISPLAY_DEVICE(); d.cb = Marshal.SizeOf(typeof(DISPLAY_DEVICE)); return d; }
        static string Explain(int r) { if (r == 0) return "Basarili"; if (r == -2) return "Ekran karti surucusu bu modu desteklemiyor."; if (r == -1) return "Genel ekran surucusu hatasi."; if (r == -3) return "Ayar kayit defterine yazilamadi."; if (r == -5) return "Yonetici yetkisi gerekli."; return "Windows ekran modu hata kodu: " + r; }
    }

    static class TimedConfirm
    {
        public static DialogResult Show(IWin32Window owner, string mode, int seconds)
        {
            using (var f = new Form { Text = "Ekran modunu koru?", ClientSize = new Size(430, 170), StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false })
            {
                var label = new Label { Location = new Point(25, 22), Size = new Size(380, 70), TextAlign = ContentAlignment.MiddleCenter };
                var yes = new Button { Text = "Koru", DialogResult = DialogResult.Yes, Location = new Point(100, 110), Size = new Size(100, 35) };
                var no = new Button { Text = "Geri al", DialogResult = DialogResult.No, Location = new Point(230, 110), Size = new Size(100, 35) };
                int left = seconds; var timer = new Timer { Interval = 1000 };
                Action update = delegate { label.Text = mode + "\n\n" + left + " saniye icinde onaylanmazsa geri alinacak."; };
                timer.Tick += delegate { left--; update(); if (left <= 0) { timer.Stop(); f.DialogResult = DialogResult.No; f.Close(); } };
                f.Controls.AddRange(new Control[] { label, yes, no }); f.AcceptButton = yes; f.CancelButton = no; update(); timer.Start(); var result = f.ShowDialog(owner); timer.Stop(); return result;
            }
        }
    }
}
