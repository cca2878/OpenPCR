using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using System;
using System.Diagnostics;

namespace OpenPCR.UICompat
{

	public class UIWidget : UIRect
	{
		public virtual Material material { get => global::OpenPCR.MigrationPending.Throw<Material>("UIWidget.get_material()", "behavior migration pending"); set => global::OpenPCR.MigrationPending.Throw("OpenPCR.UICompat.UIWidget::set_material(OpenPCR.Runtime.Api.Material):System.Void", "behavior migration pending"); }
		public virtual Texture mainTexture { get => global::OpenPCR.MigrationPending.Throw<Texture>("UIWidget.get_mainTexture()", "behavior migration pending"); set => global::OpenPCR.MigrationPending.Throw("OpenPCR.UICompat.UIWidget::set_mainTexture(OpenPCR.Runtime.Api.Texture):System.Void", "behavior migration pending"); }
		[NonSerialized]
		public UIGeometry geometry;

		public int width
		{
			get
			{
				throw new global::OpenPCR.OutOfBoundaryException("UIWidget::width");
			}

			set
			{
				throw new global::OpenPCR.OutOfBoundaryException("UIWidget::width");
			}
		}

		public Color color
		{
			get
			{
				throw new global::OpenPCR.OutOfBoundaryException("UIWidget::color");
			}

			[global::OpenPCR.Migration("dc2a014a-fa4a-43f9-a825-87ab5563a2c8")]
			set
			{
				throw new global::OpenPCR.OutOfBoundaryException("UIWidget::color");
			}
		}

		public override float alpha
		{
			[global::OpenPCR.Migration("356ad016-514e-4d26-8ba3-bbc10fd3294e")]
			get
			{
				throw new global::OpenPCR.OutOfBoundaryException("UIWidget::alpha");
			}

			[global::OpenPCR.Migration("c25bd089-8ea2-4d03-bee2-b2edf780eebd")]
			set
			{
				throw new global::OpenPCR.OutOfBoundaryException("UIWidget::alpha");
			}
		}

		public UIWidget()
		{
			throw new global::OpenPCR.MigrationPendingException("UIWidget::.ctor()", "body migration pending");
		}
	}

}
