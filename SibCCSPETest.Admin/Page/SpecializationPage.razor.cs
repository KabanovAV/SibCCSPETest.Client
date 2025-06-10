using Microsoft.AspNetCore.Components;
using MudBlazor;
using SibCCSPETest.Data;
using SibCCSPETest.ServiceBase;
using SibCCSPETest.Shared.Components;

namespace SibCCSPETest.Admin.Page
{
    public partial class SpecializationPage
    {
        [Inject]
        public IAPIService? ServiceAPI { get; set; }

        private NexusTableGrid<Specialization>? NexusTable;
        private NexusTableGridEditMode EditMode = NexusTableGridEditMode.Single;
        private NexusTableGridSelectionMode SelectMode = NexusTableGridSelectionMode.Single;

        public bool IsCrud => NexusTable != null
            && (NexusTable.InsertItem.Count > 0 || NexusTable.EditedItem.Count > 0);
        public bool IsSaveCancel => !IsCrud;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            var s = await ServiceAPI.SpecializationService.GetAllSpecialization();
            specializations = s.ToList();
        }

        public void InserRow()
            => NexusTable.InsertRow(new Specialization());

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
                    var editRow = NexusTable.SelectedRows.First();
                    NexusTable.EditRow(editRow);
                }
            }
        }

        public async Task Save()
        {
            var specialization = NexusTable.EditedItem.First();
            if (specialization.Id != 0)
                await Update(specialization);
            else
                await Add(specialization);
            NexusTable.CancelEditRow(specialization);
        }

        public async Task Add(Specialization item)
        {
            Specialization specialization = await ServiceAPI.SpecializationService.AddSpecialization(item);
            NexusTable.Data.Add(specialization);
            await NexusTable.SelectRow(specialization);
        }

        public async Task Update(Specialization item)
        {
            await ServiceAPI.SpecializationService.UpdateSpecialization(item);
        }

        public async Task Delete()
        {
            if (NexusTable != null && NexusTable.SelectedRows.Count != 0)
            {
                var delete = NexusTable.SelectedRows.First();
                await ServiceAPI.SpecializationService.DeleteSpecialization(delete.Id);
                NexusTable.RemoveRow(delete);
            }
        }

        public void Cancel()
        {
            var cancelRow = NexusTable.SelectedRows.First();
            NexusTable.CancelEditRow(cancelRow);
        }

        private bool _isCellEditMode;
        List<Specialization> specializations = [];
        Specialization specialization = new();
        private string searchString = "";
        private Specialization elementBeforeEdit;
        private Specialization selectedItem1 = null;

        private bool loading;
        public bool Loading
        {
            get => loading;
            set => loading = specializations == null ? true : false;
        }



        async Task<TableData<Specialization>> LoadServerData(TableState state, CancellationToken token)
        {
            IEnumerable<Specialization> data = await ServiceAPI.SpecializationService.GetAllSpecialization();
            return new TableData<Specialization>() { Items = data };
        }



        private void BackupItem(object element)
        {
            elementBeforeEdit = new()
            {
                Id = ((Specialization)element).Id,
                Title = ((Specialization)element).Title
            };
        }
        private void ResetItemToOriginalValues(object element)
        {
            ((Specialization)element).Id = elementBeforeEdit.Id;
            ((Specialization)element).Title = elementBeforeEdit.Title;
        }

        public void ClickEvent(object element)
            => Console.WriteLine("It's what I want!");

        private async Task AddItem()
        {
            specialization = new() { Id = specializations.Count + 1, Title = "Item" };
            specializations.Insert(0, specialization);
            //_table.SetEditingItem(specialization);
            await InvokeAsync(StateHasChanged);
        }
        private void AddNewEmployee()
        {
            // Создаем новую временную запись
            specialization = new() { Id = 0, Title = "Item" };

            // Добавляем в начало списка
            specializations.Insert(0, specialization);
            StateHasChanged();
            // Активируем режим редактирования
            //_table.SetEditingItem(specialization);

        }

        private void EditEmployee(Specialization employee)
        {
            //BackupItem(employee);
            //_table.SetEditingItem(employee);
            //_table.ReloadServerData();
        }

        private void SaveEmployee(Specialization employee)
        {


        }

        private void CancelEdit(Specialization employee)
        {
            specializations.Remove(employee);
        }

        private void DeleteEmployee(Specialization employee)
        {
            specializations.Remove(employee);
        }
    }
}
