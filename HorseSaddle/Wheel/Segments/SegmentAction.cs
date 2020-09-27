using LiruGameHelper.XML;
using System;
using System.Xml;

namespace HorseSaddle.Wheel.Segments
{
    public abstract class SegmentAction
    {
        #region XML Constants
        private const string actionAttributeName = "Action";
        #endregion

        #region Dependencies

        #endregion

        #region Fields

        #endregion

        #region Properties
        protected Saddle mainGame { get; private set; }

        public virtual bool TeamTargeted { get; } = false;
        #endregion

        #region Constructors
        protected SegmentAction(Saddle mainGame)
        {
            this.mainGame = mainGame ?? throw new ArgumentNullException(nameof(mainGame));
        }
        #endregion

        #region Do Functions
        public virtual void DoLeft() { }

        public virtual void Do() { }

        public virtual void DoRight() { }
        #endregion

        #region Load Functions
        public static SegmentAction LoadFromXMLNode(XmlNode segmentNode, Saddle mainGame)
        {
            if (segmentNode == null) throw new ArgumentNullException(nameof(segmentNode));
            if (mainGame == null) throw new ArgumentNullException(nameof(mainGame));

            if (!segmentNode.GetAttributeValue(actionAttributeName, out string actionName)) return null;

            switch (actionName)
            {
                case AddFreeUnitAction.Name:
                    return new AddFreeUnitAction(segmentNode, mainGame);
                case AddBonusPointsAction.Name:
                    return new AddBonusPointsAction(segmentNode, mainGame);
                case AddLeaderAction.Name:
                    return new AddLeaderAction(segmentNode, mainGame);
                case SwitchArmiesAction.Name:
                    return new SwitchArmiesAction(mainGame);
                default:
                    throw new Exception($"Invalid action name: {actionName}");
            }
        }
        #endregion
    }
}
