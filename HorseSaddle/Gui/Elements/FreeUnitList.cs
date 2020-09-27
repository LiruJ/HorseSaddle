using GuiCookie.Attributes;
using GuiCookie.Elements;
using HorseSaddle.Units;

namespace HorseSaddle.Gui.Elements
{
    public class FreeUnitList : Element
    {
        #region Dependencies
        private readonly AttributeManager attributeManager;
        #endregion

        #region Elements
        private TeamDisplay teamDisplay = null;

        private Button addUnitButton = null;

        private UnitSelectorWindow unitSelectorWindow = null;
        #endregion

        #region Constructors
        public FreeUnitList(AttributeManager attributeManager)
        {
            this.attributeManager = attributeManager;
        }
        #endregion

        #region Initialisation Functions
        public override void Start()
        {
            unitSelectorWindow = Root.GetElementFromTag<UnitSelectorWindow>("AddUnitWindow");

            teamDisplay = Parent as TeamDisplay;

            addUnitButton = Parent.GetChildByName<Button>("AddUnitButton", true);
            addUnitButton.ConnectLeftClick(() => unitSelectorWindow.OpenWindow(teamDisplay.Team));
        }
        #endregion

        #region List Functions
        public void Add(Unit unit)
        {

            AttributeCollection holderAttributes = new AttributeCollection(attributeManager);
            holderAttributes.Add("Size", "100%, 5%");
            holderAttributes.Add("Style", "ListItem");

            UnitListItem unitListItem = elementManager.CreateElementFromTemplateName("UnitListItem", holderAttributes, this) as UnitListItem;
            unitListItem.Populate(attributeManager, unit);
            unitListItem.OnRemoveButtonClicked.Connect(() => teamDisplay.Team.RemoveFreeUnit(unit));
        }

        public void Remove(Unit unit)
        {
            foreach (UnitListItem unitListItem in Children)
                if (unitListItem.Unit == unit)
                {
                    unitListItem.Destroy();
                    return;
                }
        }
        #endregion
    }
}