AddonInfo = {}
RecipeShareCraftskills = {}
RecipeShareTradeskills = {}

local function ResetFile()
    AddonInfo = {
        version = C_AddOns.GetAddOnMetadata('RecipeShare', 'Version'),
        name = UnitNameUnmodified("player"),
        realm = GetRealmName()
        }
    RecipeShareCraftskills = { }
    RecipeShareTradeskills = { }
end

local specIds = {
    ["Leatherworking"] = {
        [10660] = true, --Tribal https://www.wowhead.com/classic/spell=10660/tribal-leatherworking
        [10658] = true, --Elemental https://www.wowhead.com/classic/spell=10658/elemental-leatherworking
        [10656] = true, --Dragonscale https://www.wowhead.com/classic/spell=10656/dragonscale-leatherworking
    },
    ["Blacksmithing"] = {
        [9787] = {
            [17039] = true, --Master Swordsmith https://www.wowhead.com/classic/spell=17039/master-swordsmith
            [17040] = true, --Master Hammersmith https://www.wowhead.com/classic/spell=17040/master-hammersmith
            [17041] = true, --Master Axesmith https://www.wowhead.com/classic/spell=17041/master-axesmith
        },
        [9788] = true       --Armorsmith https://www.wowhead.com/classic/spell=9788/armorsmith
    },
    ["Engineering"] = {
        [20219] = true, --Gnomish https://www.wowhead.com/classic/spell=20219/gnomish-engineer
        [20222] = true, --Goblin https://www.wowhead.com/classic/spell=20222/goblin-engineer
    }
}

local function expandAllTradeSkills()
    local i = 1
    local canExpand = true

    while canExpand do
        local name, type = GetTradeSkillInfo(i)
        if type == "header" then
            ExpandTradeSkillSubClass(i)
        end
        i = i + 1

        canExpand = name ~= nil
    end
end

local function handlePlayerLogin(event)
    if not AddonInfo.version or AddonInfo.version ~= C_AddOns.GetAddOnMetadata('RecipeShare', 'Version') then
        ResetFile()
    end
end

local function handleCraftSkillShow(event)
    print("Loading craftskill...")
    local craftskill = {}
    local craft = 1
    local exists = true
    local professionName = GetCraftSkillLine(1)
    local craftSkillName, current, max = GetCraftDisplaySkillLine()
    while exists do
        local name, type = GetCraftInfo(craft)
        local link = GetCraftItemLink(craft)
        local skillInfo = {
            name = name,
            difficulty = type,
            link = link
        }
        local reagentInfo = {}
        for reagent = 1, GetCraftNumReagents(craft) do
            local reagentName, _, reagentCount = GetCraftReagentInfo(craft, reagent)
            local reagentItemLink = GetCraftReagentItemLink(craft, reagent)
            table.insert(reagentInfo, { name = reagentName, count = reagentCount, link = reagentItemLink })
        end
        skillInfo.reagents = reagentInfo
        if skillInfo.name then
            table.insert(craftskill, skillInfo)
        end
        exists = name ~= nil
        craft = craft + 1
    end
    local existingValue = RecipeShareTradeskills[professionName]
    existingValue = { name = craftSkillName, level = { current = current, max = max }, items = craftskill }
    RecipeShareCraftskills[professionName] = existingValue
    print(existingValue.name.." skill recorded successfuly! Reload your UI to save to file. (/reload)")
end


local function handleTradeSkillShow(event)
    print("Loading tradeskill...")
    local tradeskill = {}
    expandAllTradeSkills()
    local craft = 1
    local exists = true
    local tradeskillName, current, max = GetTradeSkillLine()
    local subSpecInfo = {}
    if specIds[tradeskillName] then
        for k, _ in pairs(specIds[tradeskillName]) do
            local spellKnown = IsSpellKnown(k)
            if spellKnown and k == 9787 then -- 9787 is weaponsmith, so we'll check for weapon master specIds
                for subK, _ in pairs(specIds[tradeskillName][k]) do
                    if IsSpellKnown(subK) then
                        local name = GetSpellInfo(k)
                        subSpecInfo[tradeskillName] = name
                    end
                end
            end
            if spellKnown then
                local name = GetSpellInfo(k)
                subSpecInfo[tradeskillName] = name;
            end
        end
    end
    local key = "n\\a"
    local skillsInHeader = nil
    while exists do
        local name, type = GetTradeSkillInfo(craft)
        if type == "header" then
            key = name
            if skillsInHeader ~= nil then
                tradeskill[key] = skillsInHeader
            end
            skillsInHeader = { items = {}, title = key }
        elseif type ~= "header" then
            if not skillsInHeader then skillsInHeader = { items = {}, title = key } end
            local link = GetTradeSkillItemLink(craft)
            local skillInfo = {
                name = name,
                difficulty = type,
                link = link
            }
            local cooldown = GetTradeSkillCooldown(craft)
            if (cooldown) then
                skillInfo.cooldown = {
                    current = time(),
                    delta = cooldown
                }
            end
            local reagentInfo = {}
            for reagent = 1, GetTradeSkillNumReagents(craft) do
                local reagentName, _, reagentCount = GetTradeSkillReagentInfo(craft, reagent)
                local reagentItemLink = GetTradeSkillReagentItemLink(craft, reagent)
                table.insert(reagentInfo, { name = reagentName, count = reagentCount, link = reagentItemLink })
            end
            skillInfo.reagents = reagentInfo
            if skillInfo.name then
                table.insert(skillsInHeader.items, skillInfo)
            end
        end
        exists = name ~= nil
        craft = craft + 1
    end
    tradeskill[key] = skillsInHeader
    local existingValue = RecipeShareTradeskills[tradeskillName]
    existingValue = {name = tradeskillName, level = { current = current, max = max }, items = tradeskill, subspec = nil }
    if subSpecInfo[tradeskillName] then
        existingValue.subspec = subSpecInfo[tradeskillName];
    end
    RecipeShareTradeskills[tradeskillName] = existingValue
    if existingValue.subspec then
        print(existingValue.name.." ("..existingValue.subspec..") recorded successfuly! Reload your UI to save to file. (/reload)")
    else
        print(existingValue.name.." recorded successfuly! Reload your UI to save to file. (/reload)")
    end
end

-- Register the event listener frame
local eventListener = CreateFrame("Frame", "RecipeShareMainFrame", UIParent)

local function eventHandler(self, event, ...)
    if event == "TRADE_SKILL_SHOW" then handleTradeSkillShow(event) end
    if event == "CRAFT_SHOW" then handleCraftSkillShow(event) end
    if event == "PLAYER_LOGIN" then handlePlayerLogin(event) end
end


-- function eventListener:BANKFRAME_OPENED(event)
--     purchasedBankSlots, full = GetNumBankSlots()
--     KraftieBankContents = {}

--     for base_bank_slot=1, 24 do
--         local _,itemCount,_,itemQuality,_,_,itemLink,_,_,itemId = GetContainerItemInfo(-1, base_bank_slot)
--         if (itemCount) then
--             table.insert(KraftieBankContents, { count = itemCount, id = itemId, quality = itemQuality, link = itemLink })
--         end
--     end

--     if (purchasedBankSlots > 0) then
--         for bank_container_index=NUM_BAG_SLOTS+1,NUM_BAG_SLOTS+1+purchasedBankSlots do
--             for bank_container_slot=1,GetContainerNumSlots(bank_container_index) do
--                 local _,itemCount,_,itemQuality,_,_,itemLink,_,_,itemId = GetContainerItemInfo(bank_container_index, bank_container_slot)
--                 if (itemCount) then
--                     table.insert(KraftieBankContents, { count = itemCount, id = itemId, quality = itemQuality, link = itemLink })
--                 end
--             end
--         end
--     end

--     for _,item in ipairs(KraftieBankContents) do
--         print(item.count .. "x " .. item.id)
--     end

-- end

-- Credit goes to Ketho (https://www.wowinterface.com/forums/showpost.php?p=323901&postcount=2)
local function KethoEditBox_Show(text)
    if not KethoEditBox then
        local f = CreateFrame("Frame", "KethoEditBox", UIParent, "DialogBoxFrame")
        f:SetPoint("CENTER")
        f:SetSize(600, 500)

        f:SetBackdrop({
            bgFile = "Interface\\DialogFrame\\UI-DialogBox-Background",
            edgeFile = "Interface\\PVPFrame\\UI-Character-PVP-Highlight", -- this one is neat
            edgeSize = 16,
            insets = { left = 8, right = 6, top = 8, bottom = 8 },
        })
        f:SetBackdropBorderColor(0, .44, .87, 0.5) -- darkblue

        -- Movable
        f:SetMovable(true)
        f:SetClampedToScreen(true)
        f:SetScript("OnMouseDown", function(self, button)
            if button == "LeftButton" then
                self:StartMoving()
            end
        end)
        f:SetScript("OnMouseUp", f.StopMovingOrSizing)

        -- ScrollFrame
        local sf = CreateFrame("ScrollFrame", "KethoEditBoxScrollFrame", KethoEditBox, "UIPanelScrollFrameTemplate")
        sf:SetPoint("LEFT", 16, 0)
        sf:SetPoint("RIGHT", -32, 0)
        sf:SetPoint("TOP", 0, -16)
        sf:SetPoint("BOTTOM", KethoEditBoxButton, "TOP", 0, 0)

        -- EditBox
        local eb = CreateFrame("EditBox", "KethoEditBoxEditBox", KethoEditBoxScrollFrame)
        eb:SetSize(sf:GetSize())
        eb:SetMultiLine(true)
        eb:SetAutoFocus(true) -- dont automatically focus
        eb:SetFontObject("ChatFontNormal")
        eb:SetScript("OnEscapePressed", function() f:Hide() end)
        sf:SetScrollChild(eb)

        -- Resizable
        f:SetResizable(true)

        local rb = CreateFrame("Button", "KethoEditBoxResizeButton", KethoEditBox)
        rb:SetPoint("BOTTOMRIGHT", -6, 7)
        rb:SetSize(16, 16)

        rb:SetNormalTexture("Interface\\ChatFrame\\UI-ChatIM-SizeGrabber-Up")
        rb:SetHighlightTexture("Interface\\ChatFrame\\UI-ChatIM-SizeGrabber-Highlight")
        rb:SetPushedTexture("Interface\\ChatFrame\\UI-ChatIM-SizeGrabber-Down")

        rb:SetScript("OnMouseDown", function(self, button)
            if button == "LeftButton" then
                f:StartSizing("BOTTOMRIGHT")
                self:GetHighlightTexture():Hide() -- more noticeable
            end
        end)
        rb:SetScript("OnMouseUp", function(self, button)
            f:StopMovingOrSizing()
            self:GetHighlightTexture():Show()
            eb:SetWidth(sf:GetWidth())
        end)
        f:Show()
    end

    if text then
        KethoEditBoxEditBox:SetText(text)
    end
    KethoEditBox:Show()
end

eventListener:SetScript("OnEvent", eventHandler)
eventListener:RegisterEvent("TRADE_SKILL_SHOW")
eventListener:RegisterEvent("CRAFT_SHOW")
eventListener:RegisterEvent("PLAYER_LOGIN")