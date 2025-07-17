using Microsoft.AspNetCore.Components;
using SibCCSPETest.Data;
using SibCCSPETest.ServiceBase;
using SibCCSPETest.Shared.Components;

namespace SibCCSPETest.Admin.Page
{
    public partial class QuestionPage
    {
        [Inject]
        public IAPIService? ServiceAPI { get; set; }
        [Inject]
        public INexusNotificationService? NotificationService { get; set; }
        [Inject]
        public INexusDialogService? DialogService { get; set; }

        private NexusTableGrid<QuestionDTO>? NexusTable;
        private NexusTableGridEditMode EditMode = NexusTableGridEditMode.Single;
        private NexusTableGridSelectionMode SelectMode = NexusTableGridSelectionMode.Single;

        private List<QuestionDTO>? Items;

        public bool IsCrud => NexusTable != null
            && (NexusTable.InsertItem.Count > 0 || NexusTable.EditedItem.Count > 0);
        public bool IsSelected => IsCrud || !NexusTable.IsRowsSelected;
        public bool IsSaveCancel => !IsCrud;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadData();
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task LoadData()
        {
            var q = await ServiceAPI!.QuestionService.GetAllQuestion();
            Items = q.ToList();
        }

        public async Task InserRow()
            => await NexusTable!.InsertRow(new QuestionDTO());

        public void EditRow()
        {
            if (NexusTable != null && NexusTable.SelectedRows.Count != 0)
            {
                if (EditMode == NexusTableGridEditMode.Multiple
                    && SelectMode == NexusTableGridSelectionMode.Multiple)
                {
                    foreach (var selectRow in NexusTable.SelectedRows)
                    {
                        NexusTable.EditRow(selectRow);
                    }
                }
                else
                {
                    var data = NexusTable.SelectedRows.First();
                    NexusTable.EditRow(data);
                }
            }
        }

        public async Task Save()
        {
            if (NexusTable!.InsertItem.Count == 0 && NexusTable!.EditedItem.Count > 0)
            {
                var data = NexusTable!.EditedItem.First();
                await Update(data);
            }
            else
            {
                var data = NexusTable!.InsertItem.First();
                await Add(data);
            }
            await NexusTable.Reload();
        }

        public async Task Add(QuestionDTO item)
        {
            var data = await ServiceAPI!.QuestionService.AddQuestion(item);
            if (data != null)
            {
                NexusTable!.Data.Add(data);
                await NexusTable.SelectRow(data);
                await NexusTable.OnExpandRow(data);
                NotificationService.ShowSuccess("Вопрос добавлен", "Успех");
            }
        }

        public async Task Update(QuestionDTO item)
        {
            var data = await ServiceAPI!.QuestionService.UpdateQuestion(item);
            if (data != null)
            {
                var index = NexusTable!.Data.FindIndex(s => s.Id == data.Id);
                if (index >= 0)
                    NexusTable.Data[index] = data;
                await NexusTable.SelectRow(data);
                await NexusTable.CancelEditRow(data);
                NotificationService.ShowSuccess("Вопрос изменен", "Успех");
            }
        }

        public async Task Delete()
        {
            if (NexusTable != null && NexusTable.SelectedRows.Count != 0)
            {
                var data = NexusTable.SelectedRows.First();
                var settings = new NexusDialogSetting("Удаление вопроса", $"Вопрос удалится вместе с ответами. Вы уверены, что хотите удалить \"{data.Title}\" вопрос?", "Отменить", "Удалить");
                var result = await DialogService.Show(settings);
                if (result?.Canceled == false)
                {
                    await ServiceAPI!.QuestionService.DeleteQuestion(data.Id);
                    NexusTable.RemoveRow(data);
                    NotificationService.ShowSuccess("Вопрос удален", "Успех");
                }
            }
        }

        public async Task Cancel()
        {
            var data = NexusTable!.SelectedRows.First();
            await NexusTable.CancelEditRow(data);
        }
    }
}
