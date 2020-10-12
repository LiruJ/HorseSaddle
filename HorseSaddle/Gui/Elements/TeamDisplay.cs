using GuiCookie.Elements;
using HorseSaddle.Teams;
using Microsoft.Xna.Framework;
using System;

namespace HorseSaddle.Gui.Elements
{
    public class TeamDisplay : Element
    {
        #region Dependencies
        private readonly Saddle mainGame;
        #endregion

        #region Elements
        private Button increaseScoreButton = null;
        private Button decreaseScoreButton = null;

        private Button addPointsButton = null;
        private Button removePointsButton = null;

        private TextBox nameDisplay = null;
        private ITextable scoreDisplay = null;

        private ITextable unitTotalDisplay = null;
        private ITextable bonusTotalDisplay = null;
        private ITextable baseTotalDisplay = null;
        private ITextable totalDisplay = null;

        private FreeUnitList freeUnitList = null;
        #endregion

        #region Fields
        private Team team;
        #endregion

        #region Properties
        public Team Team
        {
            get => team;
            set
            {
                if (value == null || team != null) return;

                team = value;

                if (nameDisplay != null) nameDisplay.Text = team.Name;

                // Bind the team getting a unit to updating the list.
                nameDisplay.Tint = team.Colour;
                team.OnUnitAdded.Connect(freeUnitList.Add);
                team.OnUnitRemoved.Connect(freeUnitList.Remove);
            }
        }
        #endregion

        #region Constructors
        public TeamDisplay(Saddle mainGame)
        {
            this.mainGame = mainGame ?? throw new ArgumentNullException(nameof(mainGame));
        }
        #endregion

        #region Initialisation Functions
        public override void Start()
        {
            // Get and bind the buttons.
            increaseScoreButton = GetChildByName<Button>("IncreaseScore", true);
            decreaseScoreButton = GetChildByName<Button>("DecreaseScore", true);

            increaseScoreButton.ConnectLeftClick(() => { if (team.Score < mainGame.MaxScore) { mainGame.CurrentRound++; team.Score++; } });
            decreaseScoreButton.ConnectLeftClick(() => { if (team.Score > 0) { mainGame.CurrentRound--; team.Score--; } });

            addPointsButton = GetChildByName<Button>("AddPoints", true);
            removePointsButton = GetChildByName<Button>("RemovePoints", true);

            addPointsButton.ConnectLeftClick(() => Team.BonusPoints += 250);
            removePointsButton.ConnectLeftClick(() => Team.BonusPoints -= 250);

            // Get the required display elements.
            nameDisplay= GetChildByName<TextBox>("Name", true);
            scoreDisplay = GetChildByName<TextBox>("Score", true);
            unitTotalDisplay = GetChildByName<TextBox>("UnitTotal", true);
            bonusTotalDisplay = GetChildByName<TextBox>("BonusTotal", true);
            baseTotalDisplay = GetChildByName<TextBox>("BaseTotal", true);
            totalDisplay = GetChildByName<TextBox>("Total", true);
            
            // Get the free unit list.
            freeUnitList = GetChildByName<FreeUnitList>("FreeUnits", true);
        }
        #endregion

        #region Update Functions
        protected override void Update(GameTime gameTime)
        {
            if (Team == null) return;

            scoreDisplay.Text = team.CurrentScoreName;

            unitTotalDisplay.Text = team.FreeUnitPoints.ToString();
            bonusTotalDisplay.Text = team.BonusPoints.ToString();
            baseTotalDisplay.Text = team.BasePoints.ToString();
            totalDisplay.Text = team.TotalBudget.ToString();

            increaseScoreButton.Enabled = team.Score < mainGame.MaxScore;
            decreaseScoreButton.Enabled = team.Score > 0;

            removePointsButton.Enabled = team.BonusPoints > 0;
        }
        #endregion
    }
}