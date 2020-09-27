using LiruGameHelper.XML;
using System.Xml;

namespace HorseSaddle.Wheel.Segments
{
    public class AddBonusPointsAction : SegmentAction
    {
        #region XML Constants
        private const string amountAttributeName = "Amount";
        #endregion

        #region Constants
        public const string Name = "AddBonusPoints";
        #endregion

        #region Fields
        private readonly int amount = 1;
        #endregion

        #region Properties
        public override bool TeamTargeted => true;
        #endregion

        #region Constructors
        public AddBonusPointsAction(XmlNode segmentNode, Saddle mainGame) : base(mainGame)
        {
            amount = segmentNode.ParseAttributeValue(amountAttributeName, int.Parse);
        }
        #endregion

        #region Do Functions
        public override void DoLeft() => mainGame.LeftTeam.BonusPoints += amount;

        public override void DoRight() => mainGame.RightTeam.BonusPoints += amount;
        #endregion
    }
}
