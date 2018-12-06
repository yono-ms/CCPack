using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CCPack
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CCMButton : ContentView
    {
        public CCMButton()
        {
            InitializeComponent();
        }

        private void SKCanvasView_Touch(object sender, SKTouchEventArgs e)
        {
            var view = sender as SKCanvasView;
            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    Debug.WriteLineIf(IsEnableLog, $"{GetType().Name} SKCanvasView_Touch {e.ActionType}");
                    IsPressed = true;
                    view.InvalidateSurface();
                    break;

                case SKTouchAction.Released:
                    Debug.WriteLineIf(IsEnableLog, $"{GetType().Name} SKCanvasView_Touch {e.ActionType}");
                    if (IsPressed)
                    {
                        IsPressed = false;
                        view.InvalidateSurface();
                        StartAnimationAsync(view);
                    }
                    break;

                case SKTouchAction.Exited:
                    Debug.WriteLineIf(IsEnableLog, $"{GetType().Name} SKCanvasView_Touch {e.ActionType}");
                    if (IsPressed)
                    {
                        IsPressed = false;
                        view.InvalidateSurface();
                    }
                    break;

                default:
                    break;
            }

            e.Handled = true;

        }

        private async void StartAnimationAsync(SKCanvasView view)
        {
            Debug.WriteLineIf(IsEnableLog, $"{GetType().Name} StartAnimationAsync");

            if (InProgress)
            {
                Debug.WriteLineIf(IsEnableLog, $" is busy.");
            }

            try
            {
                InProgress = true;

                // 開始点から中間までを分割する
                var quotient = (0.5f - CENTER_POSITION) / 4;

                CenterPosition = CENTER_POSITION;
                while (CenterPosition < 0.5f)
                {
                    CenterPosition += quotient;
                    view.InvalidateSurface();
                    await Task.Delay(TimeSpan.FromMilliseconds(20));
                    Debug.WriteLineIf(IsEnableLog, $" wake up. {CenterPosition}");
                }
                CenterPosition = CENTER_POSITION;
                view.InvalidateSurface();

                Clicked?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"{GetType().Name} StartAnimationAsync : {ex.ToString()}");
            }
            finally
            {
                InProgress = false;
            }
        }

        private void SKCanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            Debug.WriteLineIf(IsEnableLog, $"{GetType().Name} SKCanvasView_PaintSurface");
            var info = e.Info;
            var surface = e.Surface;
            var canvas = surface.Canvas;

            canvas.Clear();

            var delta = IsPressed ? MARGIN + 2 : MARGIN;
            var sigma = IsPressed ? SIGMA - 1 : SIGMA;
            var rect = new SKRect(info.Rect.Left + delta, info.Rect.Top + delta, info.Rect.Right - delta, info.Rect.Bottom - delta);

            using (var paint = new SKPaint())
            {
                // 非活性状態ではグレー
                var lightColor = IsEnabled ? LightColor.ToSKColor() : SKColors.WhiteSmoke;
                var darkColor = IsEnabled ? DarkColor.ToSKColor() : SKColors.LightGray;

                paint.Shader = SKShader.CreateLinearGradient(
                    new SKPoint(rect.Right, rect.Top),
                    new SKPoint(rect.Left, rect.Bottom),
                    new SKColor[] { lightColor, lightColor, darkColor },
                    new float[] { 0, CenterPosition, 1 },
                    SKShaderTileMode.Clamp);

                paint.ImageFilter = SKImageFilter.CreateDropShadow(
                    0,
                    0,
                    sigma,
                    sigma,
                    SKColors.LightGray,
                    SKDropShadowImageFilterShadowMode.DrawShadowAndForeground);

                canvas.DrawRoundRect(rect, RADIUS, RADIUS, paint);
            }
        }

        /// <summary>
        /// グラデーション開始点デフォルト
        /// </summary>
        const float CENTER_POSITION = 0.3f;
        /// <summary>
        /// グラデーション開始点
        /// </summary>
        public float CenterPosition { get; set; } = CENTER_POSITION;
        /// <summary>
        /// アニメーション中なら真
        /// </summary>
        public bool InProgress { get; set; }
        /// <summary>
        /// デフォルトマージン
        /// </summary>
        const int MARGIN = 4;
        /// <summary>
        /// デフォルトシグマ
        /// </summary>
        const float SIGMA = 2f;
        /// <summary>
        /// 角丸
        /// </summary>
        const float RADIUS = 4f;

        /// <summary>
        /// ボタン押下イベント
        /// </summary>
        public event EventHandler Clicked;

        /// <summary>
        /// ログ出力
        /// </summary>
        public bool IsEnableLog { get; set; }

        /// <summary>
        /// 押下中は真
        /// </summary>
        private bool _IsPressed;
        /// <summary>
        /// 押下中は真
        /// </summary>
        public bool IsPressed
        {
            get { return _IsPressed; }
            set { _IsPressed = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 明るい色
        /// </summary>
        public Color LightColor
        {
            get { return (Color)GetValue(LightColorProperty); }
            set { SetValue(LightColorProperty, value); }
        }
        /// <summary>
        /// 明るい色
        /// </summary>
        public static readonly BindableProperty LightColorProperty = BindableProperty.Create("LightColor", typeof(Color), typeof(CCButton), Color.White);
        /// <summary>
        /// 暗い色
        /// </summary>
        public Color DarkColor
        {
            get { return (Color)GetValue(DarkColorProperty); }
            set { SetValue(DarkColorProperty, value); }
        }
        /// <summary>
        /// 暗い色
        /// </summary>
        public static readonly BindableProperty DarkColorProperty = BindableProperty.Create("DarkColor", typeof(Color), typeof(CCButton), Color.DarkGray);
        /// <summary>
        /// ボタンテキスト
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        /// <summary>
        /// ボタンテキスト
        /// </summary>
        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(CCButton), null);
        /// <summary>
        /// ボタンテキストの色
        /// </summary>
        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }
        /// <summary>
        /// ボタンテキストの色
        /// </summary>
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create("TextColor", typeof(Color), typeof(CCButton), Color.White);
    }
}