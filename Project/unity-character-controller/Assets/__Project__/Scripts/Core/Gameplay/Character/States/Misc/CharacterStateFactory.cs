using System.Collections.Generic;

namespace Core.Gameplay.Character
{
    public class CharacterStateFactory
    {
        #region PRIVATE_VARIABLES

        private readonly CharacterStateMachine _context;
        private readonly Dictionary<CharacterState, CharacterBaseState> _states = new Dictionary<CharacterState, CharacterBaseState>();

        #endregion

        #region CONSTRUCTOR

        public CharacterStateFactory(CharacterStateMachine currentContext)
        {
            _context = currentContext;

            _states[CharacterState.Idle] = new CharacterIdleState(_context, this);
            _states[CharacterState.Walk] = new CharacterWalkState(_context, this);
            _states[CharacterState.Run] = new CharacterRunState(_context, this);
            _states[CharacterState.Jump] = new CharacterJumpState(_context, this);
            _states[CharacterState.Grounded] = new CharacterGroundedState(_context, this);
            _states[CharacterState.Fall] = new CharacterFallState(_context, this);
        }

        #endregion

        #region PROPERTIES

        public CharacterBaseState Idle => _states[CharacterState.Idle];
        public CharacterBaseState Walk => _states[CharacterState.Walk];
        public CharacterBaseState Run => _states[CharacterState.Run];
        public CharacterBaseState Jump => _states[CharacterState.Jump];
        public CharacterBaseState Grounded => _states[CharacterState.Grounded];
        public CharacterBaseState Fall => _states[CharacterState.Fall];

        #endregion
    }
}