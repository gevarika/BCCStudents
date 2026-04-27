namespace BCCStudents.Application.Services
{
    public class OperationResultContext
    {
        public List<OperationStepResult> Steps { get; set; } = new List<OperationStepResult>();

        public bool HasError => Steps.Any(s => !s.IsSuccess);
        public string FirstError => Steps.FirstOrDefault(s => !s.IsSuccess)?.Message;

        public void AddSuccess(string stepName)
        {
            Steps.Add(new OperationStepResult
            {
                Step = stepName,
                IsSuccess = true,
                Message = "OK"
            });
        }

        public void AddError(string stepName, string message)
        {
            Steps.Add(new OperationStepResult
            {
                Step = stepName,
                IsSuccess = false,
                Message = message
            });
        }
    }

    public class OperationStepResult
    {
        public string Step { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

}

