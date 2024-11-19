namespace InventorySystem
{
    public readonly struct UsageResult
    {
        public readonly bool IsError;
        public readonly string ErrorMessage;

        public bool IsSuccess => !IsError;

        /// <summary>
        /// Constructor for error. Use default Constructor for success.
        /// </summary>
        public UsageResult(string errorMessage)
        {
            IsError = false;
            ErrorMessage = errorMessage;
        }
    }
}