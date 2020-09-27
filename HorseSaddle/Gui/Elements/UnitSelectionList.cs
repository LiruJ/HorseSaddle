using GuiCookie.Attributes;
using GuiCookie.Elements;
using HorseSaddle.Units;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace HorseSaddle.Gui.Elements
{
    public class UnitSelectionList : Element
    {
        #region Dependencies
        private IReadOnlyDictionary<string, Unit> unitsByName = null;
        private readonly AttributeManager attributeManager;
        #endregion

        #region Elements
        private UnitSelectorWindow unitSelectorWindow = null;
        #endregion

        #region Fields
        private int currentIndex = 0;
        #endregion

        #region Properties
        public int MaxVisibleItems { get; private set; } = 20;

        public int CurrentIndex
        {
            get => currentIndex;
            set
            {
                if (currentIndex == value) return;

                currentIndex = MathHelper.Clamp(value, 0, MathHelper.Max(0, unitsByName.Count - MaxVisibleItems));

                recalculateList();
            }
        }
        #endregion

        #region Constructors
        public UnitSelectionList(AttributeManager attributeManager)
        {
            this.attributeManager = attributeManager;
        }
        #endregion

        #region Initialisation Functions
        public override void Start()
        {
            unitSelectorWindow = Parent as UnitSelectorWindow;

            // Create the list items.
            float height = 1.0f / MaxVisibleItems;
            for (int i = 0; i < MaxVisibleItems; i++)
            {
                AttributeCollection buttonAttributes = new AttributeCollection(attributeManager);
                buttonAttributes.Add("Size", $"100%, {height:P0}");
                buttonAttributes.Add("Position", $"0%, {i * height:P0}");
                buttonAttributes.Add("Text", i.ToString());
                buttonAttributes.Add("Style", "ListItem");

                TextButton listButton = elementManager.CreateElementFromTemplateName("TextButton", buttonAttributes, this) as TextButton;
                int iCopy = i;
                listButton.ConnectLeftClick(() => onButtonClicked(iCopy));
            }
        }

        public void PopulateFromUnits(IReadOnlyDictionary<string, Unit> unitsByName)
        {
            this.unitsByName = unitsByName;

            recalculateList();
        }

        private void recalculateList()
        {
            int elementIndex = 0;
            for (int unitIndex = CurrentIndex; unitIndex < unitsByName.Count && elementIndex < MaxVisibleItems; unitIndex++, elementIndex++)
            {
                TextButton listItem = Children[elementIndex] as TextButton;

                listItem.Enabled = true;
                listItem.Visible = true;

                listItem.Text = unitsByName.Values.ElementAt(unitIndex).Name;
            }

            for (; elementIndex < MaxVisibleItems; elementIndex++)
            {
                Children[elementIndex].Enabled = false;
                Children[elementIndex].Visible = false;
            }
        }
        #endregion

        #region Button Functions
        private void onButtonClicked(int index)
        {
            int totalIndex = CurrentIndex + index;

            unitSelectorWindow.Team.AddFreeUnit(unitsByName.Values.ElementAt(totalIndex));
        }
        #endregion
    }
}