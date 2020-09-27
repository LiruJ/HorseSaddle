using GuiCookie;
using HorseSaddle.Gui.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseSaddle.Gui
{
    public class WheelsController : Root
    {
        #region Fields

        #endregion

        #region Properties
        public TeamDisplay LeftTeamDisplay { get; private set; }

        public TeamDisplay RightTeamDisplay { get; private set; }

        public UnitSelectorWindow UnitSelectorWindow { get; private set; }
        #endregion

        #region Constructors

        #endregion

        #region Functions
        protected override void Initialise()
        {
            base.Initialise();

            UnitSelectorWindow = GetElementFromTag<UnitSelectorWindow>("AddUnitWindow");
            LeftTeamDisplay = GetElementFromTag<TeamDisplay>("LeftTeam");
            RightTeamDisplay = GetElementFromTag<TeamDisplay>("RightTeam");
        }
        #endregion
    }
}
