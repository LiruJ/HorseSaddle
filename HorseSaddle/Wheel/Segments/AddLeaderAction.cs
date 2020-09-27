using LiruGameHelper.XML;
using System.Xml;

namespace HorseSaddle.Wheel.Segments
{
    public class AddLeaderAction : SegmentAction
    {
        #region XML Constants
        private const string nameAttributeName = "Name";

        private const string unitAttributeName = "Unit";
        #endregion

        #region Constants
        public const string Name = "AddLeader";
        #endregion

        #region Fields
        private readonly string unitName = null;
        #endregion

        #region Constructors
        public AddLeaderAction(XmlNode segmentNode, Saddle mainGame) : base(mainGame)
        {
            if (!segmentNode.GetAttributeValue(unitAttributeName, out unitName)) unitName = segmentNode.GetAttributeValue(nameAttributeName);
        }
        #endregion

        #region Do Functions
        public override void Do()
        {
            mainGame.LeftTeam.AddFreeUnit(mainGame.UnitsByName[unitName]);
            mainGame.RightTeam.AddFreeUnit(mainGame.UnitsByName[unitName]);
        }
        #endregion
    }
}
