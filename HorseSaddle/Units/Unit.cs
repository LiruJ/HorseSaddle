using LiruGameHelper.XML;
using System;
using System.Collections.Generic;
using System.Xml;

namespace HorseSaddle.Units
{
    public struct Unit : IEquatable<Unit>
    {
        #region XML Constants
        private const string nameAttributeName = "Name";

        private const string costAttributeName = "Cost";
        #endregion

        #region Properties
        public string Name { get; }

        public int Cost{ get; }
        #endregion

        #region Constructors
        public Unit(string name, int cost)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Cost = cost;
        }
        #endregion

        #region Equality Functions
        public override bool Equals(object obj) => obj is Unit unit && Name == unit.Name && Cost == unit.Cost;

        public bool Equals(Unit other) => Name == other.Name && Cost == other.Cost;

        public override int GetHashCode()
        {
            int hashCode = -2002130610;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Cost.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Unit left, Unit right) => left.Equals(right);

        public static bool operator !=(Unit left, Unit right) => !(left == right);
        #endregion

        #region Load Functions
        public static Unit LoadFromXMLNode(XmlNode unitNode)
        {
            // Load the properties.
            string name = unitNode.GetAttributeValue(nameAttributeName);
            int cost = unitNode.ParseAttributeValue(costAttributeName, int.Parse);

            // Create and return the loaded unit.
            return new Unit(name, cost);
        }
        #endregion
    }
}
