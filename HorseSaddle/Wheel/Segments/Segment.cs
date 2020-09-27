using LiruGameHelper.XML;
using System.Xml;

namespace HorseSaddle.Wheel.Segments
{
    public class Segment
    {
        #region XML Constants
        private const string nameAttributeName = "Name";
        #endregion

        #region Properties
        public string Name { get; }

        public SegmentAction Action { get; }

        public bool HasAction => Action != null;
        #endregion

        #region Constructors
        public Segment(string name, SegmentAction action)
        {
            Name = name;
            Action = action;
        }
        #endregion

        #region Load Functions
        public static Segment LoadFromXMLNode(XmlNode segmentNode, Saddle mainGame)
        {
            string name = segmentNode.GetAttributeValue(nameAttributeName);
            SegmentAction segmentAction = SegmentAction.LoadFromXMLNode(segmentNode, mainGame);

            return new Segment(name, segmentAction);
        }
        #endregion
    }
}
