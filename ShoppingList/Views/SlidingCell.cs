using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Linq;

namespace ShoppingList
{
    public class SlidingCell : UITableViewCell
    {
        UIScrollView scrollView;
		UITapGestureRecognizer tapGesture;
        UIView scrollViewButtonView, scrollViewContentView;
        UIButton moreButton, deleteButton;
        UILabel scrollViewLabel;
        float catchWidth = 150f;

		public SlidingCell(string resuseIdentifier) : base(UITableViewCellStyle.Default, resuseIdentifier)
        {
			scrollView = new UIScrollView();
            scrollView.ShowsHorizontalScrollIndicator = false;
            scrollView.Delegate = new SlidingCellScrollDelegate(this);
			tapGesture = new UITapGestureRecognizer ();
			tapGesture.AddTarget (() => {
                if (scrollView.ContentOffset != PointF.Empty)
                {
                    scrollView.SetContentOffset(PointF.Empty, false);
                    return;
                }
                    
				var table = this.Superview.Superview as UITableView;
				var indexPath = table.IndexPathForCell (this);
				table.Source.RowSelected (table, indexPath);
			});
			scrollView.AddGestureRecognizer (tapGesture);
            ContentView.AddSubview(scrollView);


            scrollViewButtonView = new UIView();
            scrollView.AddSubview(scrollViewButtonView);

            moreButton = UIButton.FromType(UIButtonType.Custom);
            moreButton.BackgroundColor = UIColor.FromRGBA(0.78f, 0.78f, 0.8f, 1.0f);
            moreButton.SetTitle("More", UIControlState.Normal);
            moreButton.SetTitleColor(UIColor.White, UIControlState.Normal);
            scrollViewButtonView.AddSubview(moreButton);

            deleteButton = UIButton.FromType(UIButtonType.Custom);
            deleteButton.BackgroundColor = UIColor.FromRGBA(1.0f, 0.231f, 0.188f, 1.0f);
            deleteButton.SetTitle("Delete", UIControlState.Normal);
            deleteButton.SetTitleColor(UIColor.White, UIControlState.Normal);
            scrollViewButtonView.AddSubview(deleteButton);

            scrollViewContentView = new UIView();
            scrollViewContentView.BackgroundColor = UIColor.White;
            scrollView.AddSubview(scrollViewContentView);

            scrollViewLabel = new UILabel();
            scrollViewContentView.AddSubview(scrollViewLabel);
        }


        public override UILabel TextLabel
        {
            get
            {
                return scrollViewLabel;
            }
        }

		public UIButton DeleteButton { get { return deleteButton; } }
		public UIButton MoreButton { get { return moreButton; } }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            var b = ContentView.Bounds;

            scrollView.Frame = new RectangleF(0, 0, b.Width, b.Height);
            scrollView.ContentSize = new SizeF(b.Width + catchWidth, b.Height);

            scrollViewButtonView.Frame = new RectangleF(b.Width - catchWidth, 0, catchWidth, b.Height);

            moreButton.Frame = new RectangleF(0, 0, catchWidth / 2.0f, b.Height);
            deleteButton.Frame = new RectangleF(catchWidth / 2.0f, 0, catchWidth / 2.0f, b.Height);

            scrollViewContentView.Frame = new RectangleF(0, 0, b.Width, b.Height);

            scrollViewLabel.Frame = RectangleF.Inflate(scrollViewContentView.Bounds, -10, 0);
        }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();

            scrollView.SetContentOffset(PointF.Empty, false);
        }

        public class SlidingCellScrollDelegate : UIScrollViewDelegate
        {
            SlidingCell cell;
            public SlidingCellScrollDelegate(SlidingCell cell)
            {
                this.cell = cell;
            }

            public override void Scrolled(UIScrollView scrollView)
            {
                if (scrollView.ContentOffset.X < 0)
                {
                    scrollView.ContentOffset = PointF.Empty;
                }        

                cell.scrollViewButtonView.Frame = new RectangleF(scrollView.ContentOffset.X + (cell.Bounds.Width - cell.catchWidth), 0f, cell.catchWidth, cell.Bounds.Height);                   
            }

            public override void WillEndDragging(UIScrollView scrollView, PointF velocity, ref PointF targetContentOffset)
            {
                if (scrollView.ContentOffset.X > cell.catchWidth)
                {
                    targetContentOffset.X = cell.catchWidth;
                } else
                {
                    targetContentOffset = PointF.Empty;

                    InvokeOnMainThread(() =>
                    {
                        scrollView.SetContentOffset(PointF.Empty, true);
                    });
                }
            }  

        }
    }

}

