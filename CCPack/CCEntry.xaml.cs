using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CCPack
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CCEntry : ContentView
    {
        public CCEntry()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 最大文字数
        /// </summary>
        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }
        /// <summary>
        /// 最大文字数
        /// </summary>
        public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create("MaxLength", typeof(int), typeof(CCEntry), int.MaxValue);
        /// <summary>
        /// テキスト
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        /// <summary>
        /// テキスト
        /// </summary>
        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(CCEntry), null);
        /// <summary>
        /// プレースホルダー
        /// </summary>
        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }
        /// <summary>
        /// プレースホルダー
        /// </summary>
        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create("Placeholder", typeof(string), typeof(CCEntry), null);
        /// <summary>
        /// タイトル
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        /// <summary>
        /// タイトル
        /// </summary>
        public static readonly BindableProperty TitleProperty = BindableProperty.Create("Title", typeof(string), typeof(CCEntry), null);
        /// <summary>
        /// タイトル
        /// </summary>
        public string ErrorMessage
        {
            get { return (string)GetValue(ErrorMessageProperty); }
            set { SetValue(ErrorMessageProperty, value); }
        }
        /// <summary>
        /// タイトル
        /// </summary>
        public static readonly BindableProperty ErrorMessageProperty = BindableProperty.Create("ErrorMessage", typeof(string), typeof(CCEntry), null);
    }
}