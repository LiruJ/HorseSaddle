using LiruGameHelper.XML;
using System.Xml;

namespace HorseSaddle.Wheel.Segments
{
    public class AddFreeUnitAction : SegmentAction
    {
        #region XML Constants
        private const string nameAttributeName = "Name";

        private const string unitAttributeName = "Unit";

        private const string amountAttributeName = "Amount";
        #endregion

        #region Constants
        public const string Name = "AddFreeUnit";
        #endregion

        #region Fields
        private readonly string unitName = null;

        private readonly int amount = 1;
        #endregion

        #region Properties
        public override bool TeamTargeted => true;
        #endregion

        #region Constructors
        public AddFreeUnitAction(XmlNode segmentNode, Saddle mainGame) : base(mainGame)
        {
            if (!segmentNode.GetAttributeValue(unitAttributeName, out unitName)) unitName = segmentNode.GetAttributeValue(nameAttributeName);
            amount = (segmentNode.Attributes.GetNamedItem(amountAttributeName) is XmlAttribute amountAttribute) ? int.Parse(amountAttribute.Value) : 1;
        }
        #endregion

        #region Do Functions
        public override void DoLeft()
        {
            for (int i = 0; i < amount; i++)
                mainGame.LeftTeam.AddFreeUnit(mainGame.UnitsByName[unitName]);
        }

        public override void DoRight()
        {
            for (int i = 0; i < amount; i++)
                mainGame.RightTeam.AddFreeUnit(mainGame.UnitsByName[unitName]);
        }
        #endregion
    }
}
