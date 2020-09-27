using HorseSaddle.Teams;

namespace HorseSaddle.Wheel.Segments
{
    public class SwitchArmiesAction : SegmentAction
    {
        #region Constants
        public const string Name = "SwitchArmies";
        #endregion

        #region Constructors
        public SwitchArmiesAction(Saddle mainGame) : base(mainGame) { }
        #endregion

        #region Do Functions
        public override void Do() => Team.SwitchArmies(mainGame.LeftTeam, mainGame.RightTeam);
        #endregion
    }
}
