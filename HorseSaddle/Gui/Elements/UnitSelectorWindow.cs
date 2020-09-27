using GuiCookie.Elements;
using HorseSaddle.Teams;
using HorseSaddle.Units;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace HorseSaddle.Gui.Elements
{
    public class UnitSelectorWindow : Element
    {
        #region Dependencies

        #endregion

        #region Elements
        private UnitSelectionList unitSelectionList = null;

        private SliderBar scrollbar = null;

        private Button doneButton = null;
        #endregion

        #region Fields

        #endregion

        #region Properties
        public Team Team { get; private set; }
        #endregion

        #region Initialisation Functions
        public override void Start()
        {
            unitSelectionList = GetChildByName<UnitSelectionList>("UnitList", true);
            scrollbar = GetChildByName<SliderBar>("Scrollbar", true);
            doneButton = GetChildByName<Button>("DoneButton", true);
            doneButton.ConnectLeftClick(CloseWindow);
        }

        public override void Initialise()
        {

        }

        public override void Awake()
        {

        }
        #endregion

        #region List Functions
        public void PopulateFromUnits(IReadOnlyDictionary<string, Unit> unitsByName)
        {
            scrollbar.MaximumValue = unitsByName.Count - unitSelectionList.MaxVisibleItems;
            scrollbar.Value = 0;

            unitSelectionList.PopulateFromUnits(unitsByName);

            scrollbar.OnValueChanged.Connect(() => unitSelectionList.CurrentIndex = (int)Math.Floor(scrollbar.Value));
        }
        #endregion

        #region Window Functions
        public void OpenWindow(Team team)
        {
            Visible = true;
            Enabled = true;

            Team = team;

            foreach (Element element in elementManager)
            {
                if (element == this) continue;
                element.Enabled = false;
                element.Visible = element is TeamDisplay;
            }
        }

        public void CloseWindow()
        {
            Visible = false;
            Enabled = false;

            foreach (Element element in elementManager)
            {
                if (element == this) continue;
                element.Enabled = true;
                element.Visible = true;
            }
        }
        #endregion
    }
}