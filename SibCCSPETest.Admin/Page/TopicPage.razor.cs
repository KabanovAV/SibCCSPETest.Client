using Microsoft.AspNetCore.Components;
using SibCCSPETest.Data;
using SibCCSPETest.ServiceBase;
using SibCCSPETest.Shared.Components;

namespace SibCCSPETest.Admin.Page
{
    public partial class TopicPage
    {
        [Inject]
        public IAPIService? ServiceAPI { get; set; }
        [Inject]
        public INexusNotificationService? NotificationService { get; set; }
        [Inject]
        public INexusDialogService? DialogService { get; set; }

        private NexusTableGrid<TopicDTO>? NexusTable;
        private NexusTableGridEditMode EditMode = NexusTableGridEditMode.Single;
        private NexusTableGridSelectionMode SelectMode = NexusTableGridSelectionMode.Single;

        private List<TopicDTO>? Items;
        private IEnumerable<SelectItem>? SpecializationSelectItems;

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
            var t = await ServiceAPI!.TopicService.GetAllTopic();
            Items = t.ToList();
        }

        public async Task InserRow()
        {
            SpecializationSelectItems = await ServiceAPI.SpecializationService.GetSpecializationSelect();
            await NexusTable!.InsertRow(new TopicDTO());
        }

        public async Task EditRow()
        {
            if (NexusTable != null && NexusTable.SelectedRows.Count != 0)
            {
                SpecializationSelectItems = await ServiceAPI.SpecializationService.GetSpecializationSelect();
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

        public async Task Add(TopicDTO item)
        {
            var data = await ServiceAPI!.TopicService.AddTopic(item);
            if (data != null)
            {
                NexusTable!.Data.Add(data);
                await NexusTable.SelectRow(data);
                NotificationService.ShowSuccess("Тема добавлена", "Успех");
            }
        }

        public async Task Update(TopicDTO item)
        {
            var data = await ServiceAPI!.TopicService.UpdateTopic(item);
            if (data != null)
            {
                var index = NexusTable!.Data.FindIndex(s => s.Id == data.Id);
                if (index >= 0)
                    NexusTable.Data[index] = data;
                await NexusTable.SelectRow(data);
                await NexusTable.CancelEditRow(data);
                NotificationService.ShowSuccess("Тема изменена", "Успех");
            }
        }

        public async Task Delete()
        {
            if (NexusTable != null && NexusTable.SelectedRows.Count != 0)
            {
                var data = NexusTable.SelectedRows.First();
                var settings = new NexusDialogSetting("Удаление темы", $"Вы уверены, что хотите удалить \"{data.Title}\" тему?", "Отменить", "Удалить");
                var result = await DialogService.Show(settings);
                if (result?.Canceled == false)
                {
                    await ServiceAPI!.TopicService.DeleteTopic(data.Id);
                    NexusTable.RemoveRow(data);
                    NotificationService.ShowSuccess("Тема удалена", "Успех");
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
