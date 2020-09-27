using GuiCookie.Attributes;
using GuiCookie.Components;
using GuiCookie.Elements;
using HorseSaddle.Wheel;
using HorseSaddle.Wheel.Segments;
using System.Collections.Generic;

namespace HorseSaddle.Gui.Elements
{
    public class WheelsTabBar : Element
    {
        #region Dependencies
        private readonly AttributeManager attributeManager;
        #endregion

        #region Elements
        private WheelHolder wheelHolder = null;

        private readonly List<Button> wheelButtons = new List<Button>();
        #endregion

        #region Components
        private AspectRatioFitter aspectRatioFitter = null;

        #endregion

        #region Fields
        private List<SpinningWheel<Segment>> wheels = null;
        #endregion

        #region Properties
        public List<SpinningWheel<Segment>> Wheels
        {
            get => wheels;
            set
            {
                wheels = value;

                populateButtons();
            }
        }

        #endregion

        #region Constructors
        public WheelsTabBar(AttributeManager attributeManager, List<SpinningWheel<Segment>> wheels)
        {
            this.attributeManager = attributeManager ?? throw new System.ArgumentNullException(nameof(attributeManager));
            this.wheels = wheels ?? throw new System.ArgumentNullException(nameof(wheels));
        }
        #endregion

        #region Initialisation Functions
        public override void Start()
        {
            // Get the main wheel holder.
            wheelHolder = Root.GetElementFromTag<WheelHolder>("CurrentWheel");

            // Populate the wheel buttons.
            populateButtons();


        }

        public override void Initialise()
        {
            // Get the aspect ratio fitter.
            aspectRatioFitter = GetComponent<AspectRatioFitter>();
        }

        private void populateButtons()
        {
            int newCount = Wheels == null ? 0 : Wheels.Count;

            if (Wheels != null)
            {
                aspectRatioFitter.Ratio = Wheels.Count;
                wheelHolder.Wheel = Wheels[0];
            }

            // Work out if buttons need to be destroyed or created.
            if (newCount > wheelButtons.Count)
            {
                // Go over the new additions.
                wheelButtons.Capacity = newCount;
                for (int i = wheelButtons.Count; i < newCount; i++)
                {
                    // Create and populate attributes for this new button.
                    AttributeCollection buttonAttributes = new AttributeCollection(attributeManager);
                    buttonAttributes.Add("Style", "Inlay");
                    buttonAttributes.Add("ClippingMode", "Stretch");

                    // Create the button.
                    ImageButton newButton = elementManager.CreateElementFromTemplateName("ImageButton", buttonAttributes, this) as ImageButton;

                    // Set the icon of the button.
                    if (wheels[i].Icon != null) newButton.SetImage(wheels[i].Icon);

                    // Bind the left click of the button to activate its wheel.
                    int iCopy = i;
                    newButton.ConnectLeftClick(() => wheelHolder.Wheel = wheels[iCopy]);

                    // Add the button to the list.
                    wheelButtons.Add(newButton);
                }
            }
            else if (newCount < wheelButtons.Count)
            {
                // Destroy the old buttons.
                for (int i = newCount; i < wheelButtons.Count; i++)
                    wheelButtons[i].Destroy();
            }

            // Resize and reposition the buttons.
            for (int i = 0; i < newCount; i++)
            {
                wheelButtons[i].Bounds.ScaledSize = new GuiCookie.DataStructures.Space(1.0f / newCount, 1, GuiCookie.DataStructures.Axes.Both);
                wheelButtons[i].Bounds.ScaledPosition = new GuiCookie.DataStructures.Space((1.0f / newCount) * i, 0, GuiCookie.DataStructures.Axes.Both);
            }
        }
        #endregion
    }
}