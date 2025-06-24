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

        private NexusTableGrid<SpecializationDTO>? NexusTable;
        private NexusTableGridEditMode EditMode = NexusTableGridEditMode.Single;
        private NexusTableGridSelectionMode SelectMode = NexusTableGridSelectionMode.Single;

        private List<SpecializationDTO>? Items;
        private SpecializationDTO? Data = new();

        public bool IsCrud => NexusTable != null
            && (NexusTable.InsertItem.Count > 0 || NexusTable.EditedItem.Count > 0);
        public bool IsSelected => IsCrud || !NexusTable.IsRowsSelected();
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
                    Data = NexusTable.SelectedRows.First();
                    NexusTable.EditRow(Data);
                }
            }
        }

        public async Task Save()
        {
            Data = NexusTable!.EditedItem.First();
            if (Data.Id != 0)
                await Update(Data);
            else
                await Add(Data);
            await NexusTable.Reload();
        }

        public async Task Add(SpecializationDTO item)
        {
            Data = await ServiceAPI!.SpecializationService.AddSpecialization(item);
            if (Data != null)
            {
                NexusTable!.Data.Add(Data);
                await NexusTable.SelectRow(Data);
            }
        }

        public async Task Update(SpecializationDTO item)
        {
            await ServiceAPI!.SpecializationService.UpdateSpecialization(item);
            NexusTable.CancelEditRow(Data);
        }

        public async Task Delete()
        {
            if (NexusTable != null && NexusTable.SelectedRows.Count != 0)
            {
                Data = NexusTable.SelectedRows.First();
                await ServiceAPI!.SpecializationService.DeleteSpecialization(Data.Id);
                NexusTable.RemoveRow(Data);
            }
        }

        public void Cancel()
        {
            Data = NexusTable!.SelectedRows.First();
            NexusTable.CancelEditRow(Data);
        }
    }
}
