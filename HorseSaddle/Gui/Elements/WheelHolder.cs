using GuiCookie.Elements;
using GuiCookie.Rendering;
using HorseSaddle.Wheel;
using HorseSaddle.Wheel.Segments;
using Microsoft.Xna.Framework;
using System;

namespace HorseSaddle.Gui.Elements
{
    public class WheelHolder : Element
    {
        #region Dependencies
        private readonly Random random = null;
        private readonly Saddle mainGame;
        #endregion

        #region Elements
        private ITextable wheelNameDisplay = null;

        private ITextable segmentDisplay = null;

        private Button spinButton = null;

        private Button removeButton = null;

        private Button resetButton = null;

        private Button doButton = null;

        private Button doLeftButton = null;

        private Button doRightButton = null;

        private SpinningWheel<Segment> wheel = null;
        #endregion

        #region Fields


        #endregion

        #region Properties
        public SpinningWheel<Segment> Wheel
        {
            get => wheel;
            set
            {
                wheel = value;

                if (wheel == null) return;

                wheelNameDisplay.Text = wheel.Name;

                wheel.ScreenPosition = Bounds.AbsoluteTotalArea.Center.ToVector2();
                wheel.Radius = (ushort)Math.Max(1, Math.Floor(Bounds.ContentSize.X / 2.0f));
            }
        }
        #endregion

        #region Constructors
        public WheelHolder(Random random, Saddle mainGame)
        {
            this.random = random;
            this.mainGame = mainGame ?? throw new ArgumentNullException(nameof(mainGame));
        }
        #endregion

        #region Initialisation Functions
        public override void Start()
        {

        }

        public override void Initialise()
        {

        }

        public override void Awake()
        {
            // Get the wheel name display.
            wheelNameDisplay = Parent.GetChildByName<TextBox>("NameDisplay");

            // Get the segment display.
            segmentDisplay = Parent.GetChildByName<TextBox>("SegmentDisplay");

            // Get the buttons.
            spinButton = Parent.GetChildByName<Button>("SpinButton", true);
            removeButton = Parent.GetChildByName<Button>("RemoveButton", true);
            resetButton = Parent.GetChildByName<Button>("ResetButton", true);
            doLeftButton = Parent.GetChildByName<Button>("DoLeftButton", true);
            doButton = Parent.GetChildByName<Button>("DoButton", true);
            doRightButton = Parent.GetChildByName<Button>("DoRightButton", true);

            // Connect the buttons being clicked.
            spinButton.ConnectLeftClick(spinWheel);
            removeButton.ConnectLeftClick(removeSegment);
            resetButton.ConnectLeftClick(reloadWheel);
            doLeftButton.ConnectLeftClick(doLeftAction);
            doButton.ConnectLeftClick(doAction);
            doRightButton.ConnectLeftClick(doRightAction);
        }
        #endregion

        #region Do Functions
        private void doLeftAction()
        {
            Wheel?.CurrentSegment?.Action?.DoLeft();
            Wheel?.RemoveCurrentSegment();
        }

        private void doAction()
        {
            Wheel?.CurrentSegment?.Action?.Do();
            Wheel?.RemoveCurrentSegment();
        }

        private void doRightAction()
        {
            Wheel?.CurrentSegment?.Action?.DoRight();
            Wheel?.RemoveCurrentSegment();
        }
        #endregion

        #region Wheel Functions
        private void reloadWheel()
        {
            if (Wheel == null) return;

            Wheel.ReloadSegmentsFromFile(mainGame.WheelPresetPath);
        }

        private void spinWheel()
        {
            if (Wheel == null) return;

            Wheel.RotationalSpeed += (float)(Math.PI * (4 + (3 * random.NextDouble())));
        }

        private void removeSegment()
        {
            if (Wheel == null) return;

            Wheel.RemoveCurrentSegment();
        }
        #endregion

        #region Size Functions
        protected override void OnSizeChanged()
        {
            if (Wheel == null) return;

            // Change the radius of the wheel to the width of the holder.
            Wheel.Radius = (ushort)Math.Max(1, Math.Floor(Bounds.ContentSize.X / 2.0f));

            // Centre the wheel within the element.
            Wheel.ScreenPosition = Bounds.AbsoluteTotalArea.Center.ToVector2();
        }

        protected override void OnPositionChanged()
        {
            if (Wheel == null) return;

            // Centre the wheel within the element.
            Wheel.ScreenPosition = Bounds.AbsoluteTotalArea.Center.ToVector2();
        }
        #endregion

        #region Update Functions
        protected override void Update(GameTime gameTime)
        {
            Wheel?.Update(gameTime);

            // If the wheel is moving, disable the buttons.
            spinButton.Parent.Enabled = Wheel.IsStopped;

            // Update the current segment display.
            segmentDisplay.Text = Wheel.CurrentSegment?.Name;

            // Update the do buttons depending on the current segment.
            bool showDoButtons = Wheel.IsStopped && Wheel.CurrentSegment != null && Wheel.CurrentSegment.HasAction;

            doButton.Enabled = showDoButtons && !Wheel.CurrentSegment.Action.TeamTargeted;
            doButton.Visible = showDoButtons && !Wheel.CurrentSegment.Action.TeamTargeted;

            doLeftButton.Parent.Enabled = showDoButtons && Wheel.CurrentSegment.Action.TeamTargeted;
            doLeftButton.Parent.Visible = showDoButtons && Wheel.CurrentSegment.Action.TeamTargeted;
        }
        #endregion

        #region Draw Functions
        protected override void Draw(IGuiCamera guiCamera)
        {
            Wheel.Draw(guiCamera.SpriteBatch);
        }
        #endregion
    }
}