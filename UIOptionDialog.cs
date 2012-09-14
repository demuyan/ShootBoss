using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace ShootBoss
{
    public partial class UIOptionDialog : Dialog
    {
        public UIOptionDialog()
            : base(null, null)
        {
            InitializeWidget();
			Button_OK.ButtonAction += new EventHandler<TouchEventArgs>(Button_OK_ButtonAction);
        }
		
		public int BulletInterval
        {
           get { return (int)Slider_BulletInterval.Value; }
            set { Slider_BulletInterval.Value = value; }
        }
		
		void Button_OK_ButtonAction(object sender, TouchEventArgs e)
        {
            this.Hide();
			Game.Instance.PauseTimer = 40;
        }
    }
}
