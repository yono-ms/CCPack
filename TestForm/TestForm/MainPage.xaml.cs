using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestForm
{
    public partial class MainPage : ContentPage
    {
        public string EntryTitle { get; set; } = "お名前";
        public string EntryPlaceholder { get; set; } = "山田太郎";

        private string _Text;
        [Required(ErrorMessage = "お名前を入力してください。")]
        [MaxLength(4, ErrorMessage = "最大4文字です。")]
        public string Text
        {
            get { return _Text; }
            set { _Text = value; OnPropertyChanged(); ErrorMessage = PropertyValidation(value); }
        }

        private string _ErrorMessage;

        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value; OnPropertyChanged(); }
        }

        public MainPage()
        {
            InitializeComponent();

            Text = "";
        }

        private string PropertyValidation(object value, [CallerMemberName] string propertyName = null)
        {
            var context = new ValidationContext(this) { MemberName = propertyName };
            var results = new List<ValidationResult>();
            if (Validator.TryValidateProperty(value, context, results))
            {
                return null;
            }
            else
            {
                return string.Join("\n", results.Select(e => e.ErrorMessage));
            }
        }

        private void CCButtonPositive_Clicked(object sender, EventArgs e)
        {
            Trace.WriteLine($"CCButtonPositive_Clicked");
        }

        private void CCButtonNegative_Clicked(object sender, EventArgs e)
        {
            Trace.WriteLine($"CCButtonNegative_Clicked");
        }
    }
}
