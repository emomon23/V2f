using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace VoiceToForm.Utils
{
    public enum AlignImageEnumeration
    {
        Right,
        Left
    }

    public class ImgBtnGenerator
    {
        private Color _backColor;
        private Color _textColor;

        private static ImgBtnGenerator _buttonGeneraotr;

        private ImgBtnGenerator()
        {
            _backColor = Color.Gray;
            _textColor = Color.White;
        }

        public View CreateButton(ICommand command, double width, double height, string text, string image = null,
            double? fontSize = null, Color? backColorOverride = null)
        {
            var result = CreateButtonView(width, height, text, image, fontSize, backColorOverride);
            AddTapGestureToView(result.layout, command, text);

            return result.layout;
        }

        public View CreateButton(Func<Task> asycFunction, double width, double height, string text, string image = null,
            double? fontSize = null, Color? backColorOverride = null)
        {
            var result = CreateButtonView(width, height, text, image, fontSize, backColorOverride);
            AddTapGestureToView(result.layout, asycFunction, text);

            return result.layout;
        }

        public View CreateButton(Func<string, Task> asycFunction, double width, double height, string text, string image = null,
           double? fontSize = null, Color? backColorOverride = null, AlignImageEnumeration alignImage = AlignImageEnumeration.Left)
        {
            var result = CreateButtonView(width, height, text, image, fontSize, backColorOverride, alignImage);

            if (alignImage == AlignImageEnumeration.Left)
            {
                AddTapGestureToView(result.layout, asycFunction, text);
            }
            else
            {
                AddTapGestureToView(result, asycFunction, text);
            }

            return result.layout;
        }


        public StackLayout CreateCheckbox(StackLayout layoutFromPage, string labelText, string binding, bool initialValue, double size = 25)
        {
            //StackLayout - layout from page
            //  Stacklayout - orientation = horizaton - container
            //    Image for checkbox image
            //    Label for checkbox text

            StackLayout container = new StackLayout() {Orientation = StackOrientation.Horizontal};
           
            layoutFromPage.Children.Add(container);
            Image img = new Image()
            {
                Source = initialValue ? "checkbox.png" : "checkboxUnchecked.png",
                BackgroundColor = Color.Transparent,
                HeightRequest = size,
                WidthRequest = size
            };

            var currentValue = initialValue;
            container.Children.Add(img);

            Label lbl = new Label() {Text = labelText, HeightRequest = size + 5, FontSize = size};
            container.Children.Add(lbl);

            Switch toggle = new Switch()
            {
                HeightRequest = 1,
                WidthRequest = 1,
                IsVisible = false
            };

            container.Children.Add(toggle);
            toggle.SetBinding(Switch.IsToggledProperty, binding);

            AddTapGestureToView(container, async () =>
            {
                currentValue = !currentValue;
                img.Source = currentValue ? "checkbox.png" : "checkboxUnchecked.png";
                toggle.IsToggled = currentValue;

            }, labelText, false);

            return container;
        }

        private ImageButtonContents CreateButtonView(double width, double height, string text, string image = null,
            double? fontSize = null, Color? backColorOverride = null, AlignImageEnumeration alignImage = AlignImageEnumeration.Left)
        { 

            backColorOverride = backColorOverride.HasValue ? backColorOverride : _backColor;

            StackLayout layout = new StackLayout()
            {
                Margin = new Thickness(8,8,8,8),
                Padding = new Thickness(5,5,5,5),
                Spacing = 0,
                BackgroundColor = backColorOverride.Value,
                WidthRequest = width,
                HeightRequest = height,
                Orientation = StackOrientation.Horizontal,
                AutomationId = $"imgBtn{text}"  //Used to prevent duplicates
            };

            if (text.IsNullOrEmpty())
            {
                //No text, just an image, center in on the 'button'
                layout.VerticalOptions = LayoutOptions.CenterAndExpand;
                layout.HorizontalOptions = LayoutOptions.CenterAndExpand;
                layout.Padding = new Thickness(2,2,2,2);
            }

            Image img = null;
            if (image.IsNotNullOrEmpty())
            {
                img = new Image()
                {
                    Source = image,
                    WidthRequest = text.IsNotNullOrEmpty()? width / 5 : width,
                    HeightRequest = text.IsNotNullOrEmpty()? height / 1.8 : height,
                    BackgroundColor = Color.Transparent,
                   // Margin = new Thickness(5,0,15, 0)
                };
            }

            Label lbl = null;

            if (text.IsNotNullOrEmpty())
            {
               lbl = new Label()
                {
                    TextColor = _textColor,
                    Text = $" {text.ToUpper()}",
                    FontSize = fontSize ?? height / 2.2,
                    BackgroundColor = backColorOverride.Value,
                    HeightRequest = height - 10,
                    VerticalTextAlignment = TextAlignment.Center
                };
            }

            if (alignImage == AlignImageEnumeration.Left && img != null)
            {
                layout.Children.Add(img);
            }

            if (lbl != null)
            {
                layout.Children.Add(lbl);
            }

            if (alignImage == AlignImageEnumeration.Right && img != null)
            {
                StackLayout alignRight = new StackLayout()
                {
                    Padding = new Thickness(0, 10, 0, 0),
                    HorizontalOptions = LayoutOptions.EndAndExpand,
                    Children = { img }
                };

                layout.Children.Add(alignRight);
                layout.Padding = 0;
                layout.Margin = .5;
            }

            return new ImageButtonContents() {lbl = lbl, layout = layout, img = img};
        }

        public static void ConvertToButton(View view, ICommand command)
        {
            AddTapGestureToView(view, command, null);
        }

        private static void AddTapGestureToView(View view, ICommand command, object objText)
        {
            TapGestureRecognizer tap = new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    await view.ScaleTo(.95, 200, Easing.BounceIn);
                    view.ScaleTo(1, 200);

                    if (command != null)
                    {
                        command.Execute(objText);
                    }
                })
            };

            view.GestureRecognizers.Add(tap);
        }

        private static List<Guid> _viewsProcessing = new List<Guid>();
        private static void AddTapGestureToView(View view, Func<Task> asyncFunction, object objText, bool addClickAnimation = true)
        {
            TapGestureRecognizer tap = new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    //Don't process if the user clicks repeatedly whil we're still processing
                    if (_viewsProcessing.All(v => v != view.Id))
                    {
                       _viewsProcessing.Add(view.Id);
                        if (addClickAnimation)
                        {
                            await view.ScaleTo(.95, 200, Easing.BounceIn);
                            view.ScaleTo(1, 200);
                        }

                        await asyncFunction();
                        _viewsProcessing.Remove(view.Id);
                    }
                })
            };

            view.GestureRecognizers.Add(tap);
        }

        private static void AddTapGestureToView(View view, Func<string, Task> asyncFunction, string text)
        {
            TapGestureRecognizer tap = new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    await view.ScaleTo(.95, 200, Easing.BounceIn);
                    view.ScaleTo(1, 200);
                    await asyncFunction(text);
                })
            };

            view.GestureRecognizers.Add(tap);
        }

        private static void AddTapGestureToView(ImageButtonContents contents, Func<string, Task> asyncFunction, string text)
        {
            TapGestureRecognizer tap = new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    await contents.img.ScaleTo(.95, 200, Easing.BounceIn);
                    contents.img.ScaleTo(1, 200);
                    await asyncFunction(text);
                })
            };

            contents.layout.GestureRecognizers.Add(tap);
        }

        public static View AddButton(StackLayout container, ICommand command, double width,
                                        double height, string text = null, string image = null, double? fontSize=null, Color ? backColor=null)
        {
            //We don't add duplicates to the stacklayout
            if (container.Children.All(c => c.AutomationId != $"imgBtn{text}"))
            {
                var bg = ImgBtnGenerator.GetInstance();
                var button = bg.CreateButton(command, width, height, text, image, fontSize, backColor);
                container.Children.Add(button);
                return button;
            }

            return container.Children.FirstOrDefault(c => c.AutomationId == $"imgBtn{text}");
        }

        public static View AddButton(StackLayout container, Func<Task> asyncFunction, double width,
                                       double height, string text = null, string image = null, double? fontSize = null, Color? backColor = null)
        {
            //We don't add duplicates to the stacklayout
            if (container.Children.All(c => c.AutomationId != $"imgBtn{text}"))
            {
                var bg = ImgBtnGenerator.GetInstance();
                var button = bg.CreateButton(asyncFunction, width, height, text, image, fontSize, backColor);
                container.Children.Add(button);
                return button;
            }

            return container.Children.FirstOrDefault(c => c.AutomationId == $"imgBtn{text}");
        }

        public static View AddButton(StackLayout container, Func<string, Task> asyncFunction, double width,
                                      double height, string text = null, string image = null, double? fontSize = null, Color? backColor = null, AlignImageEnumeration alignImage = AlignImageEnumeration.Left)
        {
            //We don't add duplicates to the stacklayout
            if (container.Children.All(c => c.AutomationId != $"imgBtn{text}"))
            {
                var bg = ImgBtnGenerator.GetInstance();
                var button = bg.CreateButton(asyncFunction, width, height, text, image, fontSize, backColor, alignImage);
                container.Children.Add(button);
                return button;
            }

            return container.Children.FirstOrDefault(c => c.AutomationId == $"imgBtn{text}");
        }

        public static void BindEntryEnterKeyPress_ToButtonClick(Entry textBox, Func<Task> btnFunction)
        {
            textBox.Completed += async (object sender, EventArgs e) =>
            {
                await btnFunction();
            };
        }

        public static void BindEntryEnterKeyPress_ToButtonClick(Entry textBox, ICommand btnCommand)
        {
            textBox.Completed += (object sender, EventArgs e) =>
            {
                if (btnCommand != null)
                {
                    btnCommand.Execute(null);
                }
            };
        }

        public static ImgBtnGenerator GetInstance()
        {
            if (_buttonGeneraotr == null)
            {
                _buttonGeneraotr = new ImgBtnGenerator();
            }

            return _buttonGeneraotr;
        }

        public static ImgBtnGenerator GetInstance(Color backColor, Color texColor)
        {
            var result = GetInstance();
            result._backColor = backColor;
            result._textColor = texColor;

            return result;
        }

        internal class ImageButtonContents
        {
            public Label lbl { get; set; }
            public Image img { get; set; }
            public StackLayout layout { get; set; }
        }

    }
}
