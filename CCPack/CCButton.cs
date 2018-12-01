using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPack
{
    public class CCButton : SKCanvasView
    {
        /// <summary>
        /// グラデーション開始点デフォルト
        /// </summary>
        const float CENTER_POSITION = 0.3f;
        /// <summary>
        /// 本体マージン
        /// </summary>
        const int BODY_MARGIN = 4;

        /// <summary>
        /// ボタン押下イベント
        /// </summary>
        public event EventHandler Clicked;

        /// <summary>
        /// アニメーション中なら真
        /// </summary>
        public bool InProgress { get; set; }
        /// <summary>
        /// グラデーション開始点
        /// </summary>
        public float CenterPosition { get; set; } = CENTER_POSITION;
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
            set { _IsPressed = value; InvalidateSurface(); }
        }

        public bool IsEnableLog { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CCButton()
        {
            EnableTouchEvents = true;
        }

        protected override void OnTouch(SKTouchEventArgs e)
        {
            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    Debug.WriteLineIf(IsEnableLog, $"OnTouch {e.ActionType}");
                    IsPressed = true;
                    InvalidateSurface();
                    break;

                case SKTouchAction.Released:
                    Debug.WriteLineIf(IsEnableLog, $"OnTouch {e.ActionType}");
                    if (IsPressed)
                    {
                        IsPressed = false;
                        StartAnimationAsync();
                    }
                    break;

                case SKTouchAction.Exited:
                    Debug.WriteLineIf(IsEnableLog, $"OnTouch {e.ActionType}");
                    if (IsPressed)
                    {
                        IsPressed = false;
                        InvalidateSurface();
                    }
                    break;

                default:
                    break;
            }

            e.Handled = true;
        }

        private async void StartAnimationAsync()
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
                    InvalidateSurface();
                    await Task.Delay(TimeSpan.FromMilliseconds(20));
                    Debug.WriteLineIf(IsEnableLog, $" wake up. {CenterPosition}");
                }
                CenterPosition = CENTER_POSITION;
                InvalidateSurface();

                Clicked?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"{GetType().Name} TapGestureRecognizer_Tapped : {ex.ToString()}");
            }
            finally
            {
                InProgress = false;
            }
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            Debug.WriteLineIf(IsEnableLog, $"{GetType().Name} OnPaintSurface");
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            var bodyMargin = IsPressed ? BODY_MARGIN + 1 : BODY_MARGIN;
            var bodyRect = new SKRectI(info.Rect.Left + bodyMargin, info.Rect.Top + bodyMargin, info.Rect.Right - bodyMargin, info.Rect.Bottom - bodyMargin);

            using (var paint = new SKPaint())
            {
                // 非活性状態ではグレー
                var lightColor = IsEnabled ? LightColor.ToSKColor() : SKColors.WhiteSmoke;
                var darkColor = IsEnabled ? DarkColor.ToSKColor() : SKColors.LightGray;

                paint.Shader = SKShader.CreateLinearGradient(
                    new SKPoint(bodyRect.Right, bodyRect.Top),
                    new SKPoint(bodyRect.Left, bodyRect.Bottom),
                    new SKColor[] { lightColor, lightColor, darkColor },
                    new float[] { 0, CenterPosition, 1},
                    SKShaderTileMode.Clamp);

                if(IsEnabled)
                {
                    // 押下すると影を小さく
                    int x = IsPressed ? 0 : 1;
                    int y = IsPressed ? 0 : 1;
                    int sigmaX = IsPressed ? 1 : 2;
                    int sigmaY = IsPressed ? 1 : 2;

                    paint.ImageFilter = SKImageFilter.CreateDropShadow(
                        x,
                        y,
                        sigmaX,
                        sigmaY,
                        SKColors.LightGray,
                        SKDropShadowImageFilterShadowMode.DrawShadowAndForeground);
                }

                canvas.DrawRect(bodyRect, paint);
            }

            // 文字
            if (!string.IsNullOrEmpty(Text))
            {
                using (var paint = new SKPaint())
                {
                    //paint.Typeface = SKTypeface.FromFamilyName("Arial", SKTypefaceStyle.Bold);
                    paint.IsAntialias = true;

                    var textBounds = new SKRect();
                    paint.MeasureText(Text, ref textBounds);

                    // 高さ計算
                    float height = textBounds.Height;
                    var sizeHeight = 0.3f * info.Rect.Height * paint.TextSize / height;
                    // 幅計算
                    float width = textBounds.Width;
                    var sizeWidth = 0.7f * info.Rect.Width * paint.TextSize / width;
                    // 小さい方を採用
                    paint.TextSize = Math.Min(sizeHeight, sizeWidth);
                    paint.MeasureText(Text, ref textBounds);

                    float xText = info.Rect.Width / 2 - textBounds.MidX;
                    float yText = info.Rect.Height / 2 - textBounds.MidY;

                    // 押下時はエンボス
                    if (IsPressed)
                    {
                        canvas.Save();
                        canvas.Translate(1, 1);
                        paint.Color = SKColors.LightGray;
                        canvas.DrawText(Text, xText, yText, paint);
                        canvas.Restore();
                    }

                    paint.Color = IsEnabled ? TextColor.ToSKColor() : SKColors.Gray;
                    canvas.DrawText(Text, xText, yText, paint);
                }
            }
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
