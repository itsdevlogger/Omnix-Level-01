namespace Omnix.CharaCon
{
    public static class _
    {
        public const string MOVE_SPEED = "Move speed of the character in m/s";
        public const string SPRINT_SPEED = "Sprint speed of the character in m/s";
        public const string ROTATION_SMOOTH_TIME = "How fast the character turns to face movement direction";
        public const string SPEED_CHANGE_RATE = "Acceleration and deceleration";
        public const string GRAVITY = "The character uses its own gravity value. The engine default is -9.81f";
        public const string GROUNDED_OFFSET = "Useful for rough ground";
        public const string GROUNDED_RADIUS = "The radius of the grounded check. Should match the radius of the CharacterController";
        public const string GROUND_LAYERS = "What layers the character uses as ground";
        public const string MASS = "Player's mass.";
        public const string CINEMACHINE_CAMERA_TARGET = "The follow target set in the Cinemachine Virtual Camera that the camera will follow";
        public const string TOP_CLAMP = "How far in degrees can you move the camera up";
        public const string BOTTOM_CLAMP = "How far in degrees can you move the camera down";
        public const string CAMERA_ANGLE_OVERRIDE = "Additional degress to override the camera. Useful for fine tuning camera position when locked";
        public const string LOCK_CAMERA_POSITION = "For locking the camera position on all axis";
        public const string ABILITY_INDEX = "Integer value for AbilityIndex Animator Parameter. -ve for build-in abilities, +ve for custom abilities. 0 for none.";
        public const string ABILITY_INPUT_START_HANDLING = "Decide how the ability should behave when Ability Input Action Starts";
        public const string ABILITY_INPUT_PERFORMED_HANDLING = "Decide how the ability should behave when Ability Input Action Performed";
        public const string ABILITY_INPUT_CANCEL_HANDLING = "Decide how the ability should behave when Ability Input Action Stops";
    }
}