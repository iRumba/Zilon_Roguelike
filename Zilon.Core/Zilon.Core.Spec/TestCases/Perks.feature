﻿Feature: Perks

@perks @dev1
Scenario: Актёр игрока прокачивает перк на убийство.
	Given Есть карта размером 10
	And Есть актёр игрока класса human-person в ячейке (0, 0)
	And У актёра игрока прогресс 49 перка battle-dogmas
	And Есть монстр класса rat Id:1000 в ячейке (1, 0)
	And Монстр Id:1000 имеет Hp 1
	When Актёр игрока атакует монстра Id:1000
	Then Перк battle-dogmas должен быть прокачен
