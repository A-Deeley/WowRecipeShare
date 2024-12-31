import { useEffect, useState } from "react";
import { Item, Reagent, Tradeskill } from "./api/local/character";
import _ from "lodash";
import { DateTime, Duration } from "luxon";
import { Valid } from "luxon/src/_util";

export interface ShowTradeSkillProps {
  tradeskill: Tradeskill;
}
export function ShowTradeSkill({ tradeskill }: ShowTradeSkillProps) {
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
        width: "48%",
        maxHeight: "600px",
      }}
    >
      <div style={{ gridColumn: "span 2" }}>
        <div
          style={{
            display: "grid",
            gridTemplate: "1fr / 1fr 3fr",
            width: "50%",
            border: "2px solid gray",
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
            {tradeskill.CurrentExp} / {tradeskill.MaxExp} {tradeskill.SubSpecialisation && `(${tradeskill.SubSpecialisation})`}
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
        {Object.entries(_.groupBy(tradeskill.Items, "HeaderName")).map(
          (kvPair, index) => (
            <TradeSkillItemCategory
              selectedItemId={selectedItemId}
              key={index}
              title={kvPair[0]}
              items={kvPair[1]}
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
  selectedItem: Item;
}

export function ShowTradeSkillCraftingInfo({
  selectedItem,
}: ShowTradeSkillCraftingInfoProps) {
  console.log(selectedItem)
  let duration: Duration<Valid> | undefined;
  if (selectedItem.Cooldown){
    duration = DateTime.fromISO(selectedItem.Cooldown?.CooldownEnd).diffNow(['days', 'hours']);
  } 

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
        href={`https://classic.wowhead.com/item=${selectedItem?.ItemId}`}
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
      {duration && <span style={{ width: 'max-content', color: 'red', fontWeight: 'bold' }}>Cooldown remaining: {duration.days} Day{duration.days > 1 ? 's' : ''} {Math.floor(duration.hours)} Hr{Math.floor(duration.hours) > 1 ? 's' : ''}</span>}
      <hr style={{ gridColumn: "span 2", width: "100%" }} />
      {selectedItem?.Reagents.map((r) => (
        <TradeSkillItemReagent key={r.ItemId} reagent={r} />
      ))}
    </div>
  );
}

export interface TradeSkillItemCategoryProps {
  title: string;
  items: Item[];
  selectedItemId: number;
  updateSelectedItem: (itemId: number) => void;
}

export function TradeSkillItemCategory({
  title,
  items,
  selectedItemId,
  updateSelectedItem,
}: TradeSkillItemCategoryProps) {
  return (
    <>
      <span style={{ fontWeight: "bold", color: "orange", fontSize: "14pt" }}>
        {title}
      </span>
      <div style={{ marginLeft: "10%" }}>
        {items.map((_i, i) => (
          <TradeSkillItem
            item={_i}
            selected={_i.ItemId === selectedItemId}
            key={i}
            updateSelectedItem={updateSelectedItem}
          />
        ))}
      </div>
    </>
  );
}

export interface TradeSkillItemProps {
  item: Item;
  selected: boolean;
  updateSelectedItem: (itemId: number) => void;
}
export function TradeSkillItem({
  item,
  selected,
  updateSelectedItem,
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