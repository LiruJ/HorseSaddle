using HorseSaddle.Units;
using LiruGameHelper.Signals;
using LiruGameHelper.XML;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace HorseSaddle.Teams
{
    public class Team
    {
        #region XML Constants
        private const string nameAttributeName = "Name";

        private const string colourAttributeName = "Colour";
        #endregion

        #region Dependencies
        private readonly Saddle mainGame = null;
        #endregion

        #region Fields
        private int score;

        private readonly List<Unit> freeUnits = new List<Unit>();
        #endregion

        #region Properties
        public string Name { get; private set; }

        public int Score
        {
            get => score;
            set => score = MathHelper.Clamp(value, 0, mainGame.MaxScore);
        }

        public string CurrentScoreName => mainGame.ScoreName.Substring(0, Score);

        public Color Colour { get; set; } = Color.Black;

        public int FreeUnitPoints { get; private set; } = 0;

        public int BonusPoints { get; set; } = 0;

        public int BasePoints { get; set; } = 0;

        public int TotalBudget => BonusPoints + BasePoints + FreeUnitPoints;

        public IReadOnlyList<Unit> FreeUnits => freeUnits;
        #endregion

        #region Signals
        public IConnectableSignal<Unit> OnUnitAdded => onUnitAdded;
        private readonly Signal<Unit> onUnitAdded = new Signal<Unit>();

        public IConnectableSignal<Unit> OnUnitRemoved => onUnitRemoved;
        private readonly Signal<Unit> onUnitRemoved = new Signal<Unit>();
        #endregion

        #region Constructors
        public Team(Saddle mainGame, string name, Color colour)
        {
            this.mainGame = mainGame;

            Name = name;
            
            Colour = colour;
        }
        #endregion

        #region Switch Functions
        /// <summary> Switch all points between the two given teams. </summary>
        /// <param name="leftTeam"> The left team. </param>
        /// <param name="rightTeam"> The right team. </param>
        public static void SwitchArmies(Team leftTeam, Team rightTeam)
        {
            int leftBonus = leftTeam.BonusPoints;
            leftTeam.BonusPoints = rightTeam.BonusPoints;
            rightTeam.BonusPoints = leftBonus;

            Color leftColour = leftTeam.Colour;
            leftTeam.Colour = rightTeam.Colour;
            rightTeam.Colour = leftColour;

            // Create a copy of the teams' units.
            List<Unit> leftUnits = new List<Unit>(leftTeam.freeUnits);
            List<Unit> rightUnits = new List<Unit>(rightTeam.freeUnits);

            // Remove every unit from the left team.
            foreach (Unit leftUnit in leftUnits)
            {
                leftTeam.RemoveFreeUnit(leftUnit);
                rightTeam.AddFreeUnit(leftUnit);
            }

            // Add every unit from the left team and remove every unit from the right team.
            foreach (Unit rightUnit in rightUnits)
            {
                rightTeam.RemoveFreeUnit(rightUnit);
                leftTeam.AddFreeUnit(rightUnit);
            }
        }
        #endregion

        #region Unit Functions
        public void AddFreeUnit(Unit unit)
        {
            // Add the unit to the list.
            freeUnits.Add(unit);

            // Calculate the free unit cost total.
            FreeUnitPoints = freeUnits.Sum((u) => u.Cost);

            // Invoke the signal.
            onUnitAdded.Invoke(unit);
        }

        public void RemoveFreeUnit(Unit unit)
        {
            // Remove the unit from the list.
            if (!freeUnits.Remove(unit)) return;

            // Calculate the free unit cost total.
            FreeUnitPoints = freeUnits.Sum((u) => u.Cost);

            // Invoke the signal.
            onUnitRemoved.Invoke(unit);
        }
        #endregion

        #region Load Functions
        public static Team LoadFromXMLNode(XmlNode teamNode, Saddle mainGame)
        {
            // Load the name and colour from the node.
            string name = teamNode.GetAttributeValue(nameAttributeName);
            Color colour = teamNode.ParseAttributeValue(colourAttributeName, LiruGameHelperMonogame.Parsers.Colour.Parse);

            // Create and return the team.
            return new Team(mainGame, name, colour);
        }
        #endregion
    }
}
