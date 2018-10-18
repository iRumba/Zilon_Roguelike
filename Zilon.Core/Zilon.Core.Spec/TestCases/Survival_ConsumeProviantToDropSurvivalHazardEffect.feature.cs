﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:2.4.0.0
//      SpecFlow Generator Version:2.4.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Zilon.Core.Spec.TestCases
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.4.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [TechTalk.SpecRun.FeatureAttribute("Survival_ConsumeProviantToDropSurvivalHazardEffect", Description="\tЧтобы эмулировать восстановление сил персонажа при угрозах выживания\r\n\tКак разра" +
        "ботчику\r\n\tМне нужно, чтобы при употреблении провинта разного типа (еда/вода)\r\n\tс" +
        "брасывались соответствующие угрозы выживания при насыщении персонажа", SourceFile="TestCases\\Survival_ConsumeProviantToDropSurvivalHazardEffect.feature", SourceLine=0)]
    public partial class Survival_ConsumeProviantToDropSurvivalHazardEffectFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Survival_ConsumeProviantToDropSurvivalHazardEffect.feature"
#line hidden
        
        [TechTalk.SpecRun.FeatureInitialize()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Survival_ConsumeProviantToDropSurvivalHazardEffect", "\tЧтобы эмулировать восстановление сил персонажа при угрозах выживания\r\n\tКак разра" +
                    "ботчику\r\n\tМне нужно, чтобы при употреблении провинта разного типа (еда/вода)\r\n\tс" +
                    "брасывались соответствующие угрозы выживания при насыщении персонажа", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [TechTalk.SpecRun.FeatureCleanup()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public virtual void TestInitialize()
        {
        }
        
        [TechTalk.SpecRun.ScenarioCleanup()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void ПоглощениеПровиантаЧтобыСниматьВыживальныеСостоянияЖаждаГолод_(string stat, string statValue, string startEffect, string propSid, string provisionStat, string propCount, string expectedValue, string effect, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Поглощение провианта, чтобы снимать выживальные состояния (жажда/голод).", null, exampleTags);
#line 7
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
 testRunner.Given("Есть карта размером 2", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 9
 testRunner.And("Есть актёр игрока класса captain в ячейке (0, 0)", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 10
 testRunner.And(string.Format("Актёр значение {0} равное {1}", stat, statValue), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 11
 testRunner.And(string.Format("Актёр имеет эффект {0}", startEffect), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 12
 testRunner.And(string.Format("В инвентаре у актёра есть фейковый провиант {0} ({1})", propSid, provisionStat), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 13
 testRunner.When(string.Format("Актёр использует предмет {0} на себя", propSid), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 14
 testRunner.Then(string.Format("Значение {0} стало {1}", stat, expectedValue), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 15
 testRunner.And(string.Format("Актёр под эффектом {0}", effect), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Поглощение провианта, чтобы снимать выживальные состояния (жажда/голод)., Variant" +
            " 0", SourceLine=18)]
        public virtual void ПоглощениеПровиантаЧтобыСниматьВыживальныеСостоянияЖаждаГолод__Variant0()
        {
#line 7
this.ПоглощениеПровиантаЧтобыСниматьВыживальныеСостоянияЖаждаГолод_("сытость", "0", "Слабый голод", "fake-food", "сытость", "1", "50", "нет", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Поглощение провианта, чтобы снимать выживальные состояния (жажда/голод)., Variant" +
            " 1", SourceLine=18)]
        public virtual void ПоглощениеПровиантаЧтобыСниматьВыживальныеСостоянияЖаждаГолод__Variant1()
        {
#line 7
this.ПоглощениеПровиантаЧтобыСниматьВыживальныеСостоянияЖаждаГолод_("сытость", "-50", "Голод", "fake-food", "сытость", "1", "0", "Слабый голод", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Поглощение провианта, чтобы снимать выживальные состояния (жажда/голод)., Variant" +
            " 2", SourceLine=18)]
        public virtual void ПоглощениеПровиантаЧтобыСниматьВыживальныеСостоянияЖаждаГолод__Variant2()
        {
#line 7
this.ПоглощениеПровиантаЧтобыСниматьВыживальныеСостоянияЖаждаГолод_("сытость", "-100", "Голодание", "fake-food", "сытость", "1", "-50", "Голод", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Поглощение провианта, чтобы снимать выживальные состояния (жажда/голод)., Variant" +
            " 3", SourceLine=18)]
        public virtual void ПоглощениеПровиантаЧтобыСниматьВыживальныеСостоянияЖаждаГолод__Variant3()
        {
#line 7
this.ПоглощениеПровиантаЧтобыСниматьВыживальныеСостоянияЖаждаГолод_("вода", "0", "Слабая жажда", "fake-water", "вода", "1", "50", "нет", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Поглощение провианта, чтобы снимать выживальные состояния (жажда/голод)., Variant" +
            " 4", SourceLine=18)]
        public virtual void ПоглощениеПровиантаЧтобыСниматьВыживальныеСостоянияЖаждаГолод__Variant4()
        {
#line 7
this.ПоглощениеПровиантаЧтобыСниматьВыживальныеСостоянияЖаждаГолод_("вода", "-50", "Жажда", "fake-water", "вода", "1", "0", "Слабая жажда", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Поглощение провианта, чтобы снимать выживальные состояния (жажда/голод)., Variant" +
            " 5", SourceLine=18)]
        public virtual void ПоглощениеПровиантаЧтобыСниматьВыживальныеСостоянияЖаждаГолод__Variant5()
        {
#line 7
this.ПоглощениеПровиантаЧтобыСниматьВыживальныеСостоянияЖаждаГолод_("вода", "-100", "Обезвоживание", "fake-water", "вода", "1", "-50", "Жажда", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.TestRunCleanup()]
        public virtual void TestRunCleanup()
        {
            TechTalk.SpecFlow.TestRunnerManager.GetTestRunner().OnTestRunEnd();
        }
    }
}
#pragma warning restore
#endregion
