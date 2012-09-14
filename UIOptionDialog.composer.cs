// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace ShootBoss
{
    partial class UIOptionDialog
    {
        Label Label_1;
        Slider Slider_BulletInterval;
        Label Label_2;
        Button Button_OK;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            Slider_BulletInterval = new Slider();
            Slider_BulletInterval.Name = "Slider_BulletInterval";
            Label_2 = new Label();
            Label_2.Name = "Label_2";
            Button_OK = new Button();
            Button_OK.Name = "Button_OK";

            // Label_1
            Label_1.TextColor = new UIColor(195f / 255f, 195f / 255f, 195f / 255f, 255f / 255f);
            Label_1.Font = new UIFont( FontAlias.System, 28, FontStyle.Bold);
            Label_1.LineBreak = LineBreak.Character;
            Label_1.HorizontalAlignment = HorizontalAlignment.Center;

            // Slider_BulletInterval
            Slider_BulletInterval.MinValue = 0;
            Slider_BulletInterval.MaxValue = 100;
            Slider_BulletInterval.Value = 0;
            Slider_BulletInterval.Step = 1;

            // Label_2
            Label_2.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_2.Font = new UIFont( FontAlias.System, 25, FontStyle.Regular);
            Label_2.LineBreak = LineBreak.Character;
            Label_2.HorizontalAlignment = HorizontalAlignment.Right;

            // Button_OK
            Button_OK.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Button_OK.TextFont = new UIFont( FontAlias.System, 25, FontStyle.Regular);
            Button_OK.BackgroundFilterColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);

            // UIOptionDialog
            this.BackgroundFilterColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);

            // Dialog
            this.AddChildLast(Label_1);
            this.AddChildLast(Slider_BulletInterval);
            this.AddChildLast(Label_2);
            this.AddChildLast(Button_OK);
            this.ShowEffect = new BunjeeJumpEffect()
            {
            };
            this.HideEffect = new TiltDropEffect();

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
            case LayoutOrientation.Vertical:
                this.SetPosition(0, 0);
                this.SetSize(480, 854);
                this.Anchors = Anchors.None;

                Label_1.SetPosition(145, 61);
                Label_1.SetSize(214, 36);
                Label_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                Label_1.Visible = true;

                Slider_BulletInterval.SetPosition(397, 166);
                Slider_BulletInterval.SetSize(362, 58);
                Slider_BulletInterval.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                Slider_BulletInterval.Visible = true;

                Label_2.SetPosition(27, 175);
                Label_2.SetSize(214, 36);
                Label_2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                Label_2.Visible = true;

                Button_OK.SetPosition(319, 365);
                Button_OK.SetSize(214, 56);
                Button_OK.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                Button_OK.Visible = true;

                break;

            default:
                this.SetPosition(0, 0);
                this.SetSize(854, 480);
                this.Anchors = Anchors.None;

                Label_1.SetPosition(304, 35);
                Label_1.SetSize(245, 70);
                Label_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Right | Anchors.Width;
                Label_1.Visible = true;

                Slider_BulletInterval.SetPosition(340, 146);
                Slider_BulletInterval.SetSize(362, 58);
                Slider_BulletInterval.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                Slider_BulletInterval.Visible = true;

                Label_2.SetPosition(75, 157);
                Label_2.SetSize(229, 36);
                Label_2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                Label_2.Visible = true;

                Button_OK.SetPosition(320, 319);
                Button_OK.SetSize(214, 56);
                Button_OK.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                Button_OK.Visible = true;

                break;
            }
            _currentLayoutOrientation = orientation;
        }
        public void UpdateLanguage()
        {
            Label_1.Text = "Option";
            Label_2.Text = "BULLET INTERVAL";
            Button_OK.Text = "OK";
        }
        private void onShowing(object sender, EventArgs e)
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                {
                }
                break;

                default:
                {
                }
                break;
            }
        }
        private void onShown(object sender, EventArgs e)
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                {
                }
                break;

                default:
                {
                }
                break;
            }
        }
    }
}
