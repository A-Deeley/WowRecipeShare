import { useEffect, useState } from "react";
import { CraftItem, Craftskill, Reagent } from "./api/local/character";

export interface ShowCraftSkillProps {
  tradeskill: Craftskill;
}
export function ShowCraftSkill({ tradeskill }: ShowCraftSkillProps) {
  const [selectedItemId, setSelectedItemId] = useState<number>(
    tradeskill.Items[0].ItemId
  );
  const selectedItem = tradeskill.Items.find(
    (i) => i.ItemId === selectedItemId
  );

  const handleSelectItemChanged = (itemId: number) => {
    setSelectedItemId(itemId);
  };

  return (
    <div
      style={{
        display: "grid",
        gap: 0,
        gridTemplateRows: "5% 1fr",
        gridTemplateColumns: "1fr 1fr",
        width: "50%",
        maxHeight: "720px",
      }}
    >
      <div style={{ gridColumn: "span 2" }}>
        <div
          style={{
            display: "grid",
            gridTemplate: "1fr / 1fr 3fr",
            width: "50%",
            border: "2px solid gray",
            margin: 5,
            borderRadius: "5px",
            fontSize: "16pt",
          }}
        >
          <div
            style={{
              gridColumn: "1/span 2",
              gridRow: "1/1",
              zIndex: 1,
              background: "rgba(0, 0, 255, 0.3)",
            }}
          />
          <span
            style={{
              gridColumn: "1/1",
              gridRow: "1/1",
              zIndex: 3,
              color: "#E49900",
              fontWeight: "bold",
              marginLeft: "10%",
            }}
          >
            {tradeskill.Name}
          </span>
          <span
            style={{
              marginLeft: "10%",
              gridColumn: "2",
              gridRow: "1",
              zIndex: 3,
              color: "#E49900",
              fontWeight: "bold",
            }}
          >
            {tradeskill.CurrentExp} / {tradeskill.MaxExp}
          </span>
          <div
            style={{
              gridColumn: "1/span 2",
              gridRow: "1/1",
              fontWeight: "bold",
              background: "rgba(0, 0, 255, 1)",
              width: `${(tradeskill.CurrentExp / tradeskill.MaxExp) * 100}%`,
              zIndex: 2,
            }}
          />
        </div>
      </div>
      <div
        style={{
          gridColumn: "1",
          gridRow: "2",
          background: "#202020",
          padding: 5,
          overflow: "scroll",
        }}
      >
        {tradeskill.Items.map(
          (item) => (
            <TradeSkillItem
              key={item.ItemId}
              item={item}
              selected={item.ItemId === +selectedItemId}
              updateSelectedItem={handleSelectItemChanged}
            />
          )
        )}
      </div>
      {selectedItem && (
        <ShowTradeSkillCraftingInfo selectedItem={selectedItem} />
      )}
    </div>
  );
}

export interface ShowTradeSkillCraftingInfoProps {
  selectedItem: CraftItem;
}

export function ShowTradeSkillCraftingInfo({
  selectedItem,
}: ShowTradeSkillCraftingInfoProps) {
  return (
    <div
      key={selectedItem.ItemId}
      style={{
        height: "fit-content",
        gap: 10,
        gridColumn: "2",
        gridRow: "2",
        background: "darkgray",
        padding: 5,
        display: "grid",
        gridTemplateColumns: "1fr 1fr",
        gridTemplateRows: "auto",
      }}
    >
      <a
        target="_blank"
        style={{
          gridColumn: "span 2",
          background: "rgba(5, 5, 5, 0.6)",
          borderRadius: "10px",
          height: "fit-content",
          textDecoration: "none",
          position: "relative",
          textShadow: "#000 1px 0 4px",
        }}
        href={`https://classic.wowhead.com/spell=${selectedItem?.ItemId}`}
      >
        <span
          style={{
            position: "absolute",
            top: 0,
            left: 64,
            fontWeight: "bold",
            padding: 5,
          }}
        >
          {selectedItem?.Name}
        </span>
      </a>
      <hr style={{ gridColumn: "span 2", width: "100%" }} />
      {selectedItem?.Reagents.map((r) => (
        <TradeSkillItemReagent key={r.ItemId} reagent={r} />
      ))}
    </div>
  );
}

export interface TradeSkillItemProps {
  item: CraftItem;
  selected: boolean;
  updateSelectedItem: (itemId: number) => void;
}
export function TradeSkillItem({
  item,
  selected,
  updateSelectedItem
}: TradeSkillItemProps) {
  return (
    <div
      onClick={() => updateSelectedItem(item.ItemId)}
      style={{ fontWeight: "bold", margin: 5, padding: 2 }}
      className={`${selected ? `${item.Difficulty} Selected` : item.Difficulty}`}
    >
      {item.Name}
    </div>
  );
}

export interface TradeSkillItemReagentProps {
  reagent: Reagent;
}

export function TradeSkillItemReagent({ reagent }: TradeSkillItemReagentProps) {
  useEffect(() => {
    window.$WowheadPower.refreshLinks(true);
  });
  return (
    <a
      target="_blank"
      href={`https://classic.wowhead.com/item=${reagent.ItemId}`}
      style={{
        background: "rgba(5, 5, 5, 0.3)",
        borderRadius: "10px",
        height: "fit-content",
        textDecoration: "none",
        position: "relative",
      }}
    >
      <span style={{ position: "absolute", bottom: 0, right: 0, padding: 5 }}>
        {reagent.Count} x {reagent.Name}
      </span>
    </a>
  );
}