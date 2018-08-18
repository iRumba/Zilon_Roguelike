﻿using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using LightInject;

using Moq;

using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;

//TODO Провести реорганизацию тестов. Потому что они не актуальны после ввода абстракции намерений
namespace Zilon.Core.Tests.Tactics.Behaviour
{
    /// <summary>
    /// Тест проверяет, что источник намерений генерирует задачу после указания целевого узла.
    /// По окончанию задачи на перемещение должен выдавать пустые задачи.
    /// </summary>
    [TestFixture]
    public class HumanActorTaskSourceTests
    {
        private ServiceContainer _container;

        [Test]
        public void GetActorTasks_SelectNodeAndUpdates_ActorMovedBeforeEmptyTasks()
        {
            // ARRANGE
            var map = new TestGridGenMap();

            var startNode = map.Nodes.Cast<HexNode>().SelectBy(3, 3);
            var finishNode = map.Nodes.Cast<HexNode>().SelectBy(1, 5);

            var expectedPath = new[] {
                map.Nodes.Cast<HexNode>().SelectBy(2, 3),
                map.Nodes.Cast<HexNode>().SelectBy(2, 4),
                finishNode
            };

            var actor = CreateActor(startNode);
            var taskSource = InitTaskSource(actor);
            var moveIntetion = new MoveIntention(finishNode, map);
            taskSource.Intent(moveIntetion);

            var actorManager = CreateActorManager(actor);


            // ACT
            // ARRANGE

            // 3 шага одна и та же команда, на 4 шаг - null-комманда
            for (var step = 1; step <= 4; step++)
            {
                var tasks = taskSource.GetActorTasks(map, actorManager);

                if (step < 4)
                {
                    tasks.Length.Should().Be(1);

                    var factTask = tasks[0] as MoveTask;
                    factTask.Should().NotBeNull();

                    factTask?.IsComplete.Should().Be(false);


                    foreach (var task in tasks)
                    {
                        task.Execute();
                    }

                    actor.Node.Should().Be(expectedPath[step - 1]);
                }
                else
                {
                    tasks.Should().BeEmpty();
                }
            }

            actor.Node.Should().Be(finishNode);
        }

        private IHumanActorTaskSource InitTaskSource(IActor currentActor)
        {
            var taskSource = _container.GetInstance<IHumanActorTaskSource>();
            taskSource.SwitchActor(currentActor);
            return taskSource;
        }

        private static IDecisionSource CreateDecisionSource()
        {
            var decisionSourceMock = new Mock<IDecisionSource>();
            var decisionSource = decisionSourceMock.Object;
            return decisionSource;
        }

        private static IActor CreateActor(HexNode startNode)
        {
            var playerMock = new Mock<IPlayer>();
            var player = playerMock.Object;

            var personMock = new Mock<IPerson>();
            var person = personMock.Object;

            var actor = new Actor(person, player, startNode);
            return actor;
        }

        /// <summary>
        /// Данный метод проверяет, чтобы всегда при выдачи команды на перемещение генерировалась хотя бы одна команда.
        /// </summary>
        /// <remarks>
        /// Потому что уже команда должна разбираться, что делать, если актёр уже стоит в целевой точке.
        /// </remarks>
        [Test()]
        public void Intent_TargetToStartPoint_GenerateMoveCommand()
        {
            // ARRANGE

            var map = new TestGridGenMap();

            var startNode = map.Nodes.Cast<HexNode>().SelectBy(3, 3);

            var actor = CreateActor(startNode);

            var taskSource = InitTaskSource(actor);

            var actorManager = CreateActorManager(actor);

            var moveIntention = new MoveIntention(startNode, map);



            // ACT
            taskSource.Intent(moveIntention);



            // ASSERT
            var commands = taskSource.GetActorTasks(map, actorManager);
            commands.Should().NotBeNullOrEmpty();
            commands[0].Should().BeOfType<MoveTask>();
        }

        private static IActorManager CreateActorManager(params IActor[] actors)
        {
            var actorManagerMock = new Mock<IActorManager>();
            var actorListInner = new List<IActor>(actors);
            actorManagerMock.Setup(x => x.Actors).Returns(actorListInner);
            var actorManager = actorManagerMock.Object;
            return actorManager;
        }

        /// <summary>
        /// Тест проверяет, чтобы метод назначения задачи на перемещение проверял аргумент.
        /// Аргумент не должен быть null. Класс поведенения не знает, как в этом случае поступать.
        /// </summary>
        /// <remarks>
        /// Для отмены текущего намерения или выполняемой команды используем специальное намерение CancelIntention.
        /// </remarks>
        [Test()]
        public void Intent_SetNullTarget_ThrownException()
        {
            // ARRANGE

            var map = new TestGridGenMap();

            var startNode = map.Nodes.Cast<HexNode>().SelectBy(3, 3);

            var actor = CreateActor(startNode);

            var taskSource = InitTaskSource(actor);



            // ACT
            Action act = () => { taskSource.Intent(null); };



            // ASSERT
            act.Should().Throw<ArgumentException>();
        }

        //TODO Этот тест - это проверка намерения. Сделать отдельный тест.
        /// <summary>
        /// Тест проверяет, после окончания команды на перемещение и назнаения новой команды всё рабоает корректно.
        /// То есть новая команда возвращается при запросе.
        /// </summary>
        [Test()]
        public void Intent_AssignAfterTaskComplete_NoNullCommand()
        {
            // ARRANGE

            var map = new TestGridGenMap();

            var startNode = map.Nodes.Cast<HexNode>().SelectBy(3, 3);
            var finishNode = map.Nodes.Cast<HexNode>().SelectBy(1, 5);
            var finishNode2 = map.Nodes.Cast<HexNode>().SelectBy(3, 2);

            var actor = CreateActor(startNode);

            var taskSource = InitTaskSource(actor);

            var actorManager = CreateActorManager(actor);

            var moveIntention = new MoveIntention(finishNode, map);
            var moveIntention2 = new MoveIntention(finishNode2, map);

            // ACT

            // 1. Формируем намерение.
            taskSource.Intent(moveIntention);

            // 2. Ждём, пока команда не отработает.
            var commands = taskSource.GetActorTasks(map, actorManager);

            for (var i = 0; i < 3; i++)
            {
                foreach (var command in commands)
                {

                    command.Execute();
                }
            }

            foreach (var command in commands)
            {
                command.IsComplete.Should().Be(true);
            }

            // 3. Формируем ещё одно намерение.
            taskSource.Intent(moveIntention2);


            // 4. Запрашиваем текущие команды.
            var factCommands = taskSource.GetActorTasks(map, actorManager);




            // ASSERT
            factCommands.Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// Тест проверяет, что если указать ещё одно намерение на перемещение, пока предыдущая задача не выполнилась,
        /// то новое намерение отменяет текущее.
        /// </summary>
        [Test()]
        public void Intent_AssignTaskBeforeCurrentTaskComplete_NoNullCommand()
        {
            // ARRANGE

            var map = new TestGridGenMap();

            var startNode = map.Nodes.Cast<HexNode>().SelectBy(3, 3);
            var finishNode = map.Nodes.Cast<HexNode>().SelectBy(1, 5);
            var finishNode2 = map.Nodes.Cast<HexNode>().SelectBy(3, 2);

            var actor = CreateActor(startNode);

            var taskSource = InitTaskSource(actor);

            var actorManager = CreateActorManager(actor);

            var moveIntention = new MoveIntention(finishNode, map);
            var moveIntention2 = new MoveIntention(finishNode2, map);


            // ACT

            // 1. Формируем намерение.
            taskSource.Intent(moveIntention);

            // 2. Продвигаем выполнение текущего намерения. НО НЕ ДО ОКОНЧАНИЯ.
            var commands = taskSource.GetActorTasks(map, actorManager);

            for (var i = 0; i < 1; i++)
            {
                foreach (var command in commands)
                {

                    command.Execute();
                }
            }

            foreach (var command in commands)
            {
                command.IsComplete.Should().Be(false);
            }

            // 3. Формируем ещё одно намерение.
            taskSource.Intent(moveIntention2);


            // 4. Запрашиваем текущие команды.
            var factCommands = taskSource.GetActorTasks(map, actorManager);




            // ASSERT
            factCommands.Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// Тест проверяет, то источник задач возвращает задачу, если указать намерение атаковать.
        /// </summary>
        [Test()]
        public void IntentAttack_SetTarget_ReturnsAttackTask()
        {
            //ARRANGE
            var usageService = _container.GetInstance<ITacticalActUsageService>();

            var map = new TestGridGenMap();

            var attackerStartNode = map.Nodes.Cast<HexNode>().SelectBy(3, 3);
            var targetStartNode = map.Nodes.Cast<HexNode>().SelectBy(2, 3);


            var attackerActor = CreateActor(attackerStartNode);
            var targetActor = CreateActor(targetStartNode);

            var taskSource = InitTaskSource(attackerActor);

            var actorManager = CreateActorManager(attackerActor, targetActor);

            var attackIntention = new Intention<AttackTask>(a => new AttackTask(a, targetActor, usageService));



            // ACT
            taskSource.Intent(attackIntention);



            // ASSERT
            var tasks = taskSource.GetActorTasks(map, actorManager);

            tasks.Should().NotBeNullOrEmpty();
            tasks[0].Should().BeOfType<AttackTask>();
        }

        [Test()]
        public void IntentOpenContainer_SetContainerAndMethod_ReturnsTask()
        {
            //ARRANGE
            var map = new TestGridGenMap();

            var startNode = map.Nodes.Cast<HexNode>().SelectBy(0, 0);

            var actor = CreateActor(startNode);

            var taskSource = InitTaskSource(actor);

            var actorManager = CreateActorManager(actor);

            var containerMock = new Mock<IPropContainer>();
            var container = containerMock.Object;

            var methodMock = new Mock<IOpenContainerMethod>();
            var method = methodMock.Object;

            var intention = new Intention<OpenContainerTask>(a => new OpenContainerTask(a, container, method));



            // ACT
            taskSource.Intent(intention);



            // ASSERT
            var tasks = taskSource.GetActorTasks(map, actorManager);

            tasks.Should().NotBeNullOrEmpty();
            tasks[0].Should().BeOfType<OpenContainerTask>();
        }

        [SetUp]
        public void SetUp()
        {
            _container = new ServiceContainer();

            var decisionSource = CreateDecisionSource();

            var tacticalActUsageService = CreateTacticalActUsageService();

            _container.Register(factory => tacticalActUsageService, new PerContainerLifetime());
            _container.Register(factory => decisionSource, new PerContainerLifetime());
            _container.Register<IHumanActorTaskSource, HumanActorTaskSource>(new PerContainerLifetime());
        }

        private ITacticalActUsageService CreateTacticalActUsageService()
        {
            var tacticalActUsageServiceMock = new Mock<ITacticalActUsageService>();
            var tacticalActUsageService = tacticalActUsageServiceMock.Object;
            return tacticalActUsageService;
        }
    }
}