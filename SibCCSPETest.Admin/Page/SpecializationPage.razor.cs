using Microsoft.AspNetCore.Components;
using SibCCSPETest.Data;
using SibCCSPETest.ServiceBase;
using SibCCSPETest.Shared.Components;

namespace SibCCSPETest.Admin.Page
{
    public partial class SpecializationPage
    {
        [Inject]
        public IAPIService? ServiceAPI { get; set; }
        [Inject]
        public INexusNotificationService? NotificationService { get; set; }
        [Inject]
        public INexusDialogService? DialogService { get; set; }

        private NexusTableGrid<SpecializationDTO>? NexusTable;
        private NexusTableGridEditMode EditMode = NexusTableGridEditMode.Single;
        private NexusTableGridSelectionMode SelectMode = NexusTableGridSelectionMode.Single;

        private List<SpecializationDTO>? Items;

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
            var s = await ServiceAPI!.SpecializationService.GetAllSpecialization();
            Items = s.ToList();
        }

        public async Task InserRow()
            => await NexusTable!.InsertRow(new SpecializationDTO());

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

        public async Task Add(SpecializationDTO item)
        {
            var data = await ServiceAPI!.SpecializationService.AddSpecialization(item);
            if (data != null)
            {
                NexusTable!.Data.Add(data);
                await NexusTable.SelectRow(data);
                NotificationService.ShowSuccess("Специализация добавлена", "Успех");
            }
        }

        public async Task Update(SpecializationDTO item)
        {
            var data = await ServiceAPI!.SpecializationService.UpdateSpecialization(item);
            if (data != null)
            {
                var index = NexusTable!.Data.FindIndex(s => s.Id == data.Id);
                if (index >= 0)
                    NexusTable.Data[index] = data;
                await NexusTable.SelectRow(data);
                await NexusTable.CancelEditRow(data);
                NotificationService.ShowSuccess("Специализация изменена", "Успех");
            }
        }

        public async Task Delete()
        {
            if (NexusTable != null && NexusTable.SelectedRows.Count != 0)
            {
                var data = NexusTable.SelectedRows.First();
                var settings = new NexusDialogSetting("Удаление специализации", $"Вы уверены, что хотите удалить \"{data.Title}\" специализацию?", "Отменить", "Удалить");
                var result = await DialogService.Show(settings);
                if (result?.Canceled == false)
                {
                    var isDeleted = await ServiceAPI!.SpecializationService.DeleteSpecialization(data.Id);
                    if (isDeleted)
                    {
                        NexusTable.RemoveRow(data);
                        NotificationService.ShowSuccess("Специализация удалена", "Успех");
                    }
                    else NotificationService.ShowError("Удалить специализацию нельзя из-за наличия связей", "Ошибка");
                }
            }
        }

        public async Task Cancel()
        {
            var data = NexusTable!.SelectedRows.First();
            await NexusTable.CancelEditRow(data);
        }

        public void ShowInfoNexus()
        {
            if (NotificationService != null)
                NotificationService.ShowInfo("I'm an INFO message", "Уведомление");
        }
        public void ShowSuccessNexus()
        {
            if (NotificationService != null)
                NotificationService.ShowSuccess("I'm an SUCCESS message", "Успех");
        }
        public void ShowWarningNexus()
        {
            if (NotificationService != null)
                NotificationService.ShowWarning("I'm an WARNING message", "Предупреждение");
        }
        public void ShowErrorNexus()
        {
            if (NotificationService != null)
                NotificationService.ShowError("I'm an ERROR message", "Ошибка");
        }

        public async Task ShowDialog()
        {
            if (DialogService != null)
            {
                var settings = new NexusDialogSetting("Подтверждение", "Вы уверены, что хотите выполнить это действие?", "Отмена", "Подтвердить");
                var result = await DialogService.Show(settings);
                if (result?.Canceled == false)
                {
                    // Действие при подтверждении
                    Console.WriteLine("Пользователь подтвердил действие");
                }
                else
                {
                    // Действие при отмене
                    Console.WriteLine("Действие отменено");
                }
            }
        }
    }
}
