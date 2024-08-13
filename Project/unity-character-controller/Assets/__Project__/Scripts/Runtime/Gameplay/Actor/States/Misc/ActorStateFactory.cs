using System.Collections.Generic;

namespace Gameplay
{
    public class ActorStateFactory
    {
        #region PRIVATE_VARIABLES

        private readonly ActorStateMachine _context;
        private readonly Dictionary<ActorState, ActorBaseState> _states = new Dictionary<ActorState, ActorBaseState>();

        #endregion

        #region CONSTRUCTOR

        public ActorStateFactory(ActorStateMachine currentContext)
        {
            _context = currentContext;

            _states[ActorState.Idle] = new ActorIdleState(_context, this);
            _states[ActorState.Walk] = new ActorWalkState(_context, this);
            _states[ActorState.Run] = new ActorRunState(_context, this);
            _states[ActorState.Jump] = new ActorJumpState(_context, this);
            _states[ActorState.Grounded] = new ActorGroundedState(_context, this);
            _states[ActorState.Fall] = new ActorFallState(_context, this);
        }

        #endregion

        #region PROPERTIES

        public ActorBaseState Idle => _states[ActorState.Idle];
        public ActorBaseState Walk => _states[ActorState.Walk];
        public ActorBaseState Run => _states[ActorState.Run];
        public ActorBaseState Jump => _states[ActorState.Jump];
        public ActorBaseState Grounded => _states[ActorState.Grounded];
        public ActorBaseState Fall => _states[ActorState.Fall];

        #endregion
    }
}