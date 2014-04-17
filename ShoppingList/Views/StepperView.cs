using System;
using MonoTouch.UIKit;

namespace ShoppingList
{
	public class StepperView : UIView
	{
		private UILabel captionLabel;
		private UILabel valueLabel;
		private UIStepper stepper;

		public StepperView ():base(new System.Drawing.RectangleF(0,0,300,40))
		{
			captionLabel = new UILabel ();
			valueLabel = new UILabel ();
			stepper = new UIStepper ();
			base.AddSubview (captionLabel);
			base.AddSubview (valueLabel);
			base.AddSubview (stepper);
			valueLabel.Text = stepper.Value.ToString();
			stepper.ValueChanged += delegate {
				valueLabel.Text = stepper.Value.ToString();
			};
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			captionLabel.Frame = new System.Drawing.RectangleF (12, 5, 75, 25);
			valueLabel.Frame = new System.Drawing.RectangleF (100, 5, 100, 25);
			stepper.Frame = new System.Drawing.RectangleF (200, 5, 100, 25);
		}

//		public UIStepper Stepper { get { return stepper; } }

		public string Caption { get { return captionLabel.Text; } set { captionLabel.Text = value; } }

		public int Value 
		{ 
			get { return (int)stepper.Value; } 
			set 
			{
				if ((int)stepper.Value == value)
					return;
				stepper.Value = value;
				valueLabel.Text = value.ToString();

			}
		}
	}
}

