// DPIScalableByCodeForm.cs

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;



namespace AdvancedStep
{
    /*
        UIControlScaler class

        🧩 개요 (Overview)
          - Windows Forms 애플리케이션의 DPI 인식(Per-Monitor DPI-Aware) 지원을 위해 설계된 고급 유틸리티 클래스 이다.
          - 런타임 DPI 변경(예: 100% → 150% 또는 150% → 100%) 시점에서
            모든 컨트롤의 크기, 폰트, 여백, 위치 등을 자동으로 재조정(Rescale) 하여 일관된 UI 비율을 유지한다.
          - .NET Framework 4.7 이상의 환경에서 System DPI Awareness API(GetDpiForWindow, DpiChanged 이벤트 등)를 직접 활용하며,
            디자이너 시점의 DPI(기본 96)를 기준으로 런타임 DPI 비율을 계산해 UI를 다시 배치 한다.

        ✅ 목적
          - Windows 10/11에서 다양한 디스플레이 배율(예: 100%, 125%, 150%)로 실행될 때
            UI의 왜곡, 잘림, 폰트 크기 불균형 문제를 방지하는 것이 주된 목적이다.
          - 전통적인 WinForms는 디자인 시점 DPI(보통 96 DPI) 에 종속적이므로,
            실행 환경의 DPI가 변경되면 다음과 같은 문제가 발생한다:

            * 컨트롤 크기, 위치가 비정상적으로 커지거나 작아짐
            * 폰트 크기와 버튼 텍스트 비율이 어긋남
            * TabControl, DataGridView 등의 복합 컨트롤이 영역 밖으로 벗어남
            * 사용자 DPI 변경(예: 150% → 100%) 시 폼 하단 여백 및 정렬이 깨짐

          - UIControlScaler는 이러한 문제를 자동 보정하기 위해 각 컨트롤의 초기 상태(ControlState) 를 캐시하고,
            DPI 변경 시점마다 이를 정확히 재적용(apply) 하도록 설계되었다.

        🧱 주요 처리
          - 각 컨트롤 등록 시(register) 내부적으로 현재 DPI를 GetDpiForWindow로 획득하고,
            기준 비율(newDpi / OriginalDpi)을 계산해 사용한다.
          - 폼의 DpiChanged 이벤트가 발생하면, 전체 자식 컨트롤에 대해 재스케일(applyWithDpi)을 수행한다.

        ✅ 스케일링
          - Windows Form 부분적인 자동 스케일링 지원
          - WPF(Windows Presentation Foundation)나 WinUI로 전환하면 자동 스케일링 완벽 지원 !!!
          - AutoScaleMode.None 으로 설정하고 변화된 스케일링값을 계산하여
            종속된 모든 컨트롤들의 크기와 위치 재설정하는 것도 좋은 방법 !!!
        
          1. AutoScaleMode.Dpi 속성

            1.1. 기능
              - DPI를 기준으로 자동 스케일링
              1.1.1. 장점
                - Per-Monitor V2 완벽 지원
                - 멀티 모니터 환경에서 이동시 자동 스케일링
                - 고해상도 환경에서 정확한 레이아웃
              1.1.2. 단점
                - .NET 5+에서만 완전 동작
                - .NET Framework 에서는 제한적 (Per-Monitor V2 안됨)
    
            1.2. 자동 스케일링이 되는 것들
              | 컨트롤                           
              |----------------------------------
              | Control.Font
              | Control.Size
              | Control.Location
              | Control.Padding
              | Control.Margin
              | Form.ClientSize
              | PictureBox (Zoom, StretchImage)
              | Button, Label, TextBox, ComboBox
              | FlowLayoutPanel, TableLayoutPanel
          
            1.3. 자동 스케일링이 되지 않는 것들
              | 컨트롤                           
              |----------------------------------
              | ListView.Column.Width
              | ListView.ImageList.ImageSize
              | DataGridView.RowTemplate.Height
              | DataGridView.Column.Width
              | ToolStrip.ImageScalingSize
              | TreeView.ItemHeight

          2. AutoScaleMode.Font 속성

            2.1. 기능
              - 폰트 크기(Font.SizeInPoints)를 기준으로 자동 스케일링
              2.1.1. 장점
                - 폰트 크기 변경시 즉시 작동
                - .NET Framework 에서는 기본값으로 설정
              2.1.2. 단점
                - DPI 변경 시 불완전.
                - 멀티 모니터 환경에서 이동시 스케일링 안됨.
                - 일부 컨트롤들은 수동 조정 필요.

            2.2. 자동 스케일링이 되는 것들
              | 컨트롤                           
              |----------------------------------
              | Control.Font
              | Control.Size
              | Control.Location
              | Control.Padding
              | Control.Margin
              | Form.ClientSize
              | PictureBox (Zoom, StretchImage)
          
            2.3. 자동 스케일링이 되지 않는 것들
              | 컨트롤                           
              |----------------------------------
              | ListView.Column.Width
              | ListView.ImageList.ImageSize
              | DataGridView.RowTemplate.Height
              | DataGridView.Column.Width
              | ToolStrip.ImageScalingSize
              | SplitContainer.SplitterDistance
              | TreeView.ItemHeight
    */

    public sealed class UIControlScaler
    {
        public const int DESIGN_DPI = 96; // 디자이너가 사용한 기준 DPI

        public static readonly UIControlScaler Instance = new UIControlScaler();

        private readonly ConcurrentDictionary<Control, ControlStateCache> _caches = new ConcurrentDictionary<Control, ControlStateCache>();

        private UIControlScaler()
        {
        }

        // 모든 Control 지원!
        public void register(Control control)
        {
            if (control == null) return;

            var cache = new ControlStateCache(control);
            _caches[control] = cache;

            // 핸들이 이미 생성된 경우 즉시 캡처
            if (control.IsHandleCreated)
            {
                cache.capture();
                rescale(control, getCurrentDpi(control));
            }

            // HandleCreated에서 캡처 (아직 핸들이 없는 경우를 위해)
            control.HandleCreated += (s, e) =>
            {
                cache.capture(); // 핸들 생성 시점에 캡처
                rescale(control, getCurrentDpi(control));
            };

            var form = control as Form;
            if (null != form)
            {
                form.DpiChanged += (s, e) =>
                {
                    // ⭐ Form과 등록된 모든 자식 컨트롤을 함께 리스케일
                    rescaleControlsWithDpi(control, e.DeviceDpiNew);
                };
            }

            ControlStateCache csCache;
            control.Disposed += (s, e) => _caches.TryRemove(control, out csCache);
        }

        private void rescaleControlsWithDpi(Control control, int newDpi)
        {
            // 1. 메인 컨트롤 리스케일
            ControlStateCache csCache;
            if (_caches.TryGetValue(control, out csCache))
            {
                csCache.applyWithDpi(newDpi);
            }
        }

        public void rescale(Control control, int newDpi)
        {
            ControlStateCache csCache;
            if (_caches.TryGetValue(control, out csCache))
                csCache.applyWithDpi(newDpi);
        }

        public void rescale(Form form, int newDpi)
        {
            ControlStateCache csCache;
            if (!_caches.TryGetValue(form, out csCache)) return;
            csCache.applyWithDpi(newDpi);
        }

        private int getCurrentDpi(Control c) =>
                c.IsHandleCreated ? (int)GetDpiForWindow(c.Handle) : 96;

        public ControlStateCache getControlDpiCache(Control control) { return _caches[control]; }

        [DllImport("user32.dll")]
        public static extern uint GetDpiForWindow(IntPtr hwnd);
    }

    public class ControlStateCache
    {
        private readonly Control _control;
        private readonly Dictionary<Control, ControlState> _states = new Dictionary<Control, ControlState>();

        private bool _captured = false; // 캡처 상태 추적

        public ControlStateCache(Control control) { _control = control; }

        public void capture()
        {
            // ⭐ 이미 캡처된 경우 재캡처
            _states.Clear();

            int controlCount = 0;
            foreach (Control c in _control.getAllControls())
            {
                // ⭐ Disposed된 컨트롤 스킵
                if (c.IsDisposed) continue;

                // 각 컨트롤의 캡처 시점 DPI를 우선 GetDpiForWindow로 시도, 실패 시 Graphics.DpiX 사용
                int captureDpi = 96;
                try
                {
                    if (c.IsHandleCreated)
                    {
                        var dpiVal = UIControlScaler.GetDpiForWindow(c.Handle);
                        captureDpi = dpiVal == 0 ? 96 : (int)dpiVal;
                    }
                    else
                    {
                        using (var g = c.CreateGraphics())
                            captureDpi = (int)g.DpiX;
                    }
                }
                catch
                {
                    try
                    {
                        using (var g = c.CreateGraphics())
                            captureDpi = (int)g.DpiX;
                    }
                    catch
                    {
                        captureDpi = 96;
                    }
                }

                var f = c.Font ?? SystemFonts.DefaultFont;

                var state = new ControlState
                {
                    OriginalDpi = captureDpi,
                    Name = c.Name,
                    Text = c.Text,
                    X = c.Location.X,
                    Y = c.Location.Y,
                    W = c.Width,
                    H = c.Height,
                    FontFamily = f.FontFamily.Name,
                    FontPt = f.SizeInPoints,
                    MinimumSize = c.MinimumSize,
                    MaximumSize = c.MaximumSize,
                    Padding = c.Padding,
                    Margin = c.Margin,
                    Anchor = c.Anchor,
                    Dock = c.Dock
                };

                var dgv = c as DataGridView;
                if (null != dgv)
                {
                    state.ColumnWidths = dgv.Columns
                        .Cast<DataGridViewColumn>()
                        .Select(col => (float)col.Width)
                        .ToArray();

                    state.RowHeight = dgv.RowTemplate.Height;
                    state.ColumnHeadersHeight = dgv.ColumnHeadersHeight;
                }

                var ts = c as ToolStrip;
                if (null != ts)
                {
                    state.ToolStripItemSize = ts.ImageScalingSize;
                }

                _states[c] = state;
                controlCount++;
            }

            _captured = true;
            Debug.WriteLine($"[DPI] ✅ Captured {controlCount} controls for '{_control.GetType().Name}' ({_control.Text ?? _control.Name})");
        }

        public void applyWithDpi(int newDpi)
        {
            // ⭐ 캡처되지 않은 경우 먼저 캡처
            if (!_captured || _states.Count == 0)
            {
                Debug.WriteLine($"[DPI] Cache not captured, capturing now for {_control.Text ?? _control.Name}");
                capture();
            }

            _control.SuspendLayout();

            float ratio = newDpi / 96f;

            foreach (var kv in _states)
            {
                var c = kv.Key;
                var s = kv.Value;

                // Disposed된 컨트롤 스킵
                if (c.IsDisposed)
                {
                    continue;
                }

                // 로그 식별자
                string name = string.IsNullOrEmpty(c.Name) ? $"<{c.GetType().Name}>" : c.Name;
                string text = c.Text;

                // 기존 값 백업 (변경 감지용)
                var oldLoc = c.Location;
                var oldSize = c.Size;
                var oldFontPt = c.Font.SizeInPoints;
                var oldPadding = c.Padding;
                var oldMargin = c.Margin;


                float ratioDelta = s.OriginalDpi > 0 ? (newDpi / (float)s.OriginalDpi) : 1f;

                // 컨트롤 크기를 조정 한다.
                c.Size = new Size((int)Math.Round(s.W * ratio), (int)Math.Round(s.H * ratio));

                // 폰트 크기는 변화량으로만 조정 한다.
                c.Font = new Font(s.FontFamily, (s.FontPt * ratioDelta), c.Font.Style, GraphicsUnit.Point);

                c.Padding = s.Padding.scale(ratio);
                c.Margin = s.Margin.scale(ratio);

                c.Anchor = s.Anchor;
                c.Dock = s.Dock;

                if(null != s.MinimumSize) c.MinimumSize = s.MinimumSize.scale(ratio);
                if(null != s.MaximumSize) c.MaximumSize = s.MaximumSize.scale(ratio);

                // 특수 컨트롤 처리
                var dgv = c as DataGridView;
                if (null != dgv && s.ColumnWidths != null)
                {
                    // 컬럼 너비
                    for (int i = 0; i < Math.Min(dgv.Columns.Count, s.ColumnWidths.Length); i++)
                    {
                        int newW = (int)(s.ColumnWidths[i] * ratio);
                        if (dgv.Columns[i].Width != newW)
                        {
                            dgv.Columns[i].Width = newW;
                        }
                    }

                    if (0 < s.RowHeight)
                    {
                        int newRowHeight = (int)Math.Round(s.RowHeight * ratio);
                        dgv.RowTemplate.Height = newRowHeight;
                        dgv.RowTemplate.MinimumHeight = newRowHeight;
                    }

                    if (0 < s.ColumnHeadersHeight)
                    {
                        int newHeaderHeight = (int)Math.Round(s.ColumnHeadersHeight * ratio);
                        dgv.ColumnHeadersHeight = newHeaderHeight;
                    }
                }

                var ts = c as ToolStrip;
                if (null != ts && s.ToolStripItemSize.HasValue)
                {
                    var newSize = s.ToolStripItemSize.Value.scale(ratio);
                    if (ts.ImageScalingSize != newSize)
                    {
                        ts.ImageScalingSize = newSize;
                    }
                }
            }

            // 레이아웃 재개 및 강제 갱신
            _control.ResumeLayout(false);
            _control.PerformLayout();
            _control.Invalidate(true);
            _control.Update();
        }
    }

    [Serializable]
    public sealed class ControlState
    {
        public int OriginalDpi { get; set; } = 96;

        public string Name { get; set; }
        public string Text { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }
        public string FontFamily { get; set; }
        public float FontPt { get; set; }
        public Padding Padding { get; set; }
        public Padding Margin { get; set; }
        public AnchorStyles Anchor { get; set; } = AnchorStyles.Top | AnchorStyles.Left;
        public DockStyle Dock { get; set; } = DockStyle.None;
        public Size MinimumSize { get; set; }
        public Size MaximumSize { get; set; }
        public float[] ColumnWidths { get; set; }
        public int RowHeight { get; set; }
        public int ColumnHeadersHeight { get; set; }
        public Size? ToolStripItemSize { get; set; }
    }

    public static class UIHelper
    {
        public static IEnumerable<Control> getAllControls(this Control control)
        {
            var stack = new Stack<Control>();
            stack.Push(control);

            while (stack.Count > 0)
            {
                var c = stack.Pop();
                yield return c;

                for (int i = c.Controls.Count - 1; i >= 0; i--)
                    if (c.Controls[i] != null)
                        stack.Push(c.Controls[i]);
            }
        }

        public static Padding scale(this Padding p, float ratio)
        {
            return new Padding( (int)(p.Left * ratio)
                              , (int)(p.Top * ratio)
                              , (int)(p.Right * ratio)
                              , (int)(p.Bottom * ratio) );
        }

        public static Size scale(this Size s, float ratio)
        {
            return new Size( (int)(s.Width * ratio)
                           , (int)(s.Height * ratio) );
        }

        public static string toPointString(this Point p) => $"({p.X},{p.Y})";

        public static string toSizeString(this Size s) => $"({s.Width}x{s.Height})";

        public static string toPaddingString(this Padding p) => $"L{p.Left},T{p.Top},R{p.Right},B{p.Bottom}";

        public static bool equalPaddings(Padding a, Padding b) =>
            a.Left == b.Left && a.Top == b.Top && a.Right == b.Right && a.Bottom == b.Bottom;
    }

    public class CustomForm : Form
    {
        public CustomForm()
        {
            Text = "SDK-style DPI-aware sample";
            ClientSize = new Size(800, 480);

            var lbl = new Label { Text = "Hello — SDK-style DPI sample", Location = new Point(12, 12), AutoSize = true };
            var dgv = new DataGridView { Location = new Point(12, 50), Size = new Size(760, 360) };

            // sample columns
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Col A", Width = 200 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Col B", Width = 300 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Col C", Width = 200 });

            // add test rows
            for (int i = 0; i < 10; i++)
                dgv.Rows.Add($"A{i}", $"B{i}", $"C{i}");

            Controls.Add(lbl);
            Controls.Add(dgv);

            UIControlScaler.Instance.register(this);
        }
    }

    /*
    
    //=============================================================================================
    // .NET Framework 4.6
    //=============================================================================================
    //---------------------------------------------------------------------------------------------
    // 실행 코드
    //---------------------------------------------------------------------------------------------
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new CustomForm());
        }
    }
    //---------------------------------------------------------------------------------------------
    // app.manifest 설정
    //---------------------------------------------------------------------------------------------
    <?xml version="1.0" encoding="utf-8"?>
    <assembly xmlns="urn:schemas-microsoft-com:asm.v1" manifestVersion="1.0">
        <assemblyIdentity version="1.0.0.0" name="MyApp.app"/>
        <description>MyApp</description>

        <!-- .NET Framework 레거시 방식: 윈도우의 기본 DPI 인식 활성화 -->
        <application>
            <windowsSettings>
                <!-- true 또는 true/pm 를 지정하는 문법이 다양한데, 안전한 호환을 위해 true 사용 -->
                <dpiAware>true</dpiAware>
            </windowsSettings>
        </application>
    </assembly>
    //---------------------------------------------------------------------------------------------
    // csproj 설정
    //---------------------------------------------------------------------------------------------
    <?xml version="1.0" encoding="utf-8"?>
    <Project DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
        <PropertyGroup>
            ...
            <!-- 타깃 프레임워크: 반드시 4.6 이상 -->
            <TargetFrameworkVersion>v4.6</TargetFrameworkVersion>
            <!-- 매니페스트 지정: 빌드 시 EXE에 병합됩니다 -->
            <ApplicationManifest>app.manifest</ApplicationManifest>
            ...
        </PropertyGroup>
        <ItemGroup>
            ...
            <None Include="app.manifest" />
            ...
        </ItemGroup>
    </Project>
    //=============================================================================================


    //=============================================================================================
    // .NET Framework 4.7 with PerMonitorV2
    //=============================================================================================
    //---------------------------------------------------------------------------------------------
    // 실행 코드
    //---------------------------------------------------------------------------------------------
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // .NET Framework 4.7부터는 프레임워크가 개선되어
            // Per-monitor 관련 동작이 더 나아집니다.
            Application.Run(new CustomForm());
        }
    }
    //---------------------------------------------------------------------------------------------
    // .NET Framework 4.7 Manifest 사용할 경우 (app.manifest) with PerMonitorV2
    //---------------------------------------------------------------------------------------------
    <?xml version="1.0" encoding="utf-8"?>
    <assembly xmlns="urn:schemas-microsoft-com:asm.v1" manifestVersion="1.0"
              xmlns:asmv3="urn:schemas-microsoft-com:asm.v3">
        <assemblyIdentity version="1.0.0.0" name="MyApp.app"/>
        <description>MyApp</description>

        <asmv3:application>
            <asmv3:windowsSettings>
                <!-- Per-monitor v2: 가능한 경우 OS/프레임워크에서 더 나은 런타임 스케일링 제공 -->
                <dpiAwareness>PerMonitorV2</dpiAwareness>
            </asmv3:windowsSettings>
        </asmv3:application>
    </assembly>
    //---------------------------------------------------------------------------------------------
    // csproj 설정
    //---------------------------------------------------------------------------------------------
    <?xml version="1.0" encoding="utf-8"?>
    <Project DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
        <PropertyGroup>
            ...
            <!-- 타깃 프레임워크: 반드시 4.7 이상 -->
            <TargetFrameworkVersion>v4.7.2</TargetFrameworkVersion>
            <!-- 매니페스트 지정: 빌드 시 EXE에 병합됩니다 -->
            <ApplicationManifest>app.manifest</ApplicationManifest>
            ...
        </PropertyGroup>
        <ItemGroup>
            ...
            <None Include="app.manifest" />
            ...
        </ItemGroup>
    </Project>
    //=============================================================================================


    //=============================================================================================
    // .NET Framework 4.8, .NET Core, .NET 5+ with PerMonitorV2
    //=============================================================================================
    //---------------------------------------------------------------------------------------------
    // 실행 코드
    //---------------------------------------------------------------------------------------------
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // .NET Framework 4.8, .NET Core, .NET 5 이상에서는 앱 시작시 HighDpiMode를 설정
            // Manifest 를 사용할 경우 SetHighDpiMode(HighDpiMode.PerMonitorV2) 는 제거해야 한다 !!!
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new CustomForm());
        }
    }
    //---------------------------------------------------------------------------------------------
    // .NET 5+ Manifest 사용할 경우 (app.manifest) with PerMonitorV2
    //---------------------------------------------------------------------------------------------
    <?xml version="1.0" encoding="utf-8"?>
    <assembly xmlns="urn:schemas-microsoft-com:asm.v1" manifestVersion="1.0"
              xmlns:asmv3="urn:schemas-microsoft-com:asm.v3">
        <assemblyIdentity version="1.0.0.0" name="MyApp.NET6.app"/>
        <description>My .NET 6 WinForms App</description>

        <asmv3:application>
            <asmv3:windowsSettings>
                <dpiAwareness>PerMonitorV2</dpiAwareness>
            </asmv3:windowsSettings>
        </asmv3:application>
    </assembly>
    //---------------------------------------------------------------------------------------------
    // csproj 설정
    //---------------------------------------------------------------------------------------------
    <Project Sdk="Microsoft.NET.Sdk.WindowsDesktop">
        <PropertyGroup>
            ...
            <TargetFramework>net5.0-windows</TargetFramework>
            <UseWindowsForms>true</UseWindowsForms>

            <!-- 매니페스트 지정 (선택) -->
            <ApplicationManifest>app.manifest</ApplicationManifest>
            ...
        </PropertyGroup>

        <ItemGroup>
            ...
            <None Include="app.manifest" />
            ...
        </ItemGroup>
    </Project>
    //=============================================================================================

    */
}
