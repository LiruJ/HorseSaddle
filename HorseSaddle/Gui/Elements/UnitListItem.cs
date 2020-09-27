using GuiCookie.Attributes;
using GuiCookie.Elements;
using HorseSaddle.Units;
using LiruGameHelper.Signals;

namespace HorseSaddle.Gui.Elements
{
    public class UnitListItem : Element
    {
        #region Dependencies

        #endregion

        #region Elements
        private Button removeButton = null;
        #endregion

        #region Fields
        public Unit Unit { get; private set; }
        #endregion

        #region Properties

        #endregion

        #region Signals
        public IConnectableSignal OnRemoveButtonClicked => removeButton.LeftClicked;
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

        }

        public void Populate(AttributeManager attributeManager, Unit unit)
        {
            Unit = unit;

            AttributeCollection nameAttributes = new AttributeCollection(attributeManager);
            nameAttributes.Add("Size", "70%, 100%");
            nameAttributes.Add("Text", unit.Name);
            nameAttributes.Add("Style", "JustText");
            elementManager.CreateElementFromTemplateName("TextBox", nameAttributes, this);

            AttributeCollection costAttributes = new AttributeCollection(attributeManager);
            costAttributes.Add("Size", "20%, 100%");
            costAttributes.Add("Position", "70%, 0%");
            costAttributes.Add("Text", unit.Cost);
            costAttributes.Add("Style", "JustText");
            elementManager.CreateElementFromTemplateName("TextBox", costAttributes, this);

            AttributeCollection buttonAttributes = new AttributeCollection(attributeManager);
            buttonAttributes.Add("Size", "10%, 100%");
            buttonAttributes.Add("Position", "95%, 0%");
            buttonAttributes.Add("Pivot", "50%, 0");
            buttonAttributes.Add("Style", "Remove");
            buttonAttributes.Add("Ratio", "1");
            buttonAttributes.Add("RatioMode", "HeightControlsWidth");
            buttonAttributes.Add("HoveredStyle", "RemoveHover");
            buttonAttributes.Add("ClickedStyle", "RemoveClick");
            removeButton = elementManager.CreateElementFromTemplateName("SquareButton", buttonAttributes, this) as Button;
        }
        #endregion

        #region Functions

        #endregion
    }
}