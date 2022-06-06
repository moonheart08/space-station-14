using JetBrains.Annotations;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Chemistry.Components;

namespace Content.Shared.Chemistry.EntitySystems
{
    [UsedImplicitly]
    public abstract class SharedChemMasterSystem : EntitySystem
    {
        [Dependency] private readonly ItemSlotsSystem _itemSlotsSystem = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<SharedChemMasterComponent, ComponentStartup>(OnComponentInit);
            SubscribeLocalEvent<SharedChemMasterComponent, ComponentShutdown>(OnComponentRemove);
        }

        private void OnComponentInit(EntityUid uid, SharedChemMasterComponent component, ComponentStartup args)
        {
            _itemSlotsSystem.AddItemSlot(uid, SharedChemMasterComponent.BeakerSlotId, component.BeakerSlot);
        }

        private void OnComponentRemove(EntityUid uid, SharedChemMasterComponent component, ComponentShutdown args)
        {
            _itemSlotsSystem.RemoveItemSlot(uid, component.BeakerSlot);
        }
    }
}
