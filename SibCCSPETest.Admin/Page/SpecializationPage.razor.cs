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

        private List<SpecializationDTO> Specializations = [];
        private SpecializationDTO? Specialization = new();

        public bool IsCrud => NexusTable != null
            && (NexusTable.InsertItem.Count > 0 || NexusTable.EditedItem.Count > 0);
        public bool IsSaveCancel => !IsCrud;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            var s = await ServiceAPI!.SpecializationService.GetAllSpecialization();
            Specializations = s.ToList();
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
                    Specialization = NexusTable.SelectedRows.First();
                    NexusTable.EditRow(Specialization);
                }
            }
        }

        public async Task Save()
        {
            Specialization = NexusTable!.EditedItem.First();
            if (Specialization.Id != 0)
                await Update(Specialization);
            else
                await Add(Specialization);
            await NexusTable.Reload();
        }

        public async Task Add(SpecializationDTO item)
        {
            Specialization = await ServiceAPI!.SpecializationService.AddSpecialization(item);
            if (Specialization != null)
            {
                NexusTable!.Data.Add(Specialization);
                await NexusTable.SelectRow(Specialization);                
            }
        }

        public async Task Update(SpecializationDTO item)
        {
            await ServiceAPI!.SpecializationService.UpdateSpecialization(item);
            NexusTable.CancelEditRow(Specialization);
        }

        public async Task Delete()
        {
            if (NexusTable != null && NexusTable.SelectedRows.Count != 0)
            {
                Specialization = NexusTable.SelectedRows.First();
                await ServiceAPI!.SpecializationService.DeleteSpecialization(Specialization.Id);
                NexusTable.RemoveRow(Specialization);
            }
        }

        public void Cancel()
        {
            Specialization = NexusTable!.SelectedRows.First();
            NexusTable.CancelEditRow(Specialization);
        }
    }
}
