using Microsoft.AspNetCore.Mvc.ModelBinding;

public class ErrorMessage
{
    /// <summary>取得資料驗證錯誤清單
    /// </summary>
    /// <param name="ModelState">資料狀態</param>
    /// <returns>驗證錯誤訊息清單</returns>
    public List<string> ErrorMessageToList(ModelStateDictionary ModelState)
    {
        List<string> errorList = new List<string>();
        foreach (KeyValuePair<string, ModelStateEntry> state in ModelState)
        {
            if (state.Value.Errors.Count > 0)
            {
                foreach (ModelError error in state.Value.Errors)
                {
                    errorList.Add(error.ErrorMessage);
                }
            }
        }
        return errorList;
    }
}