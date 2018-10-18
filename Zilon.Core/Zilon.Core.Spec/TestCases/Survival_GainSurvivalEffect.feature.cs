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
    [TechTalk.SpecRun.FeatureAttribute("Survival_GainSurvivalEffect", Description="\tЧтобы ввести микроменеджмент ресурсов и состояния персонажей\r\n\tКак игроку\r\n\tМне " +
        "нужно, чтобы каждый ход значение сытости персонажа падало", SourceFile="TestCases\\Survival_GainSurvivalEffect.feature", SourceLine=0)]
    public partial class Survival_GainSurvivalEffectFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Survival_GainSurvivalEffect.feature"
#line hidden
        
        [TechTalk.SpecRun.FeatureInitialize()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Survival_GainSurvivalEffect", "\tЧтобы ввести микроменеджмент ресурсов и состояния персонажей\r\n\tКак игроку\r\n\tМне " +
                    "нужно, чтобы каждый ход значение сытости персонажа падало", ProgrammingLanguage.CSharp, ((string[])(null)));
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
        
        public virtual void НаступлениеВыживальныхСостоянийЖаждаГолодУтомление(string moveDistance, string stat, string expectedValue, string effect, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "survival",
                    "dev0"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Наступление выживальных состояний (жажда/голод/утомление)", null, @__tags);
#line 7
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
 testRunner.Given("Есть карта размером 2", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 9
 testRunner.And("Есть актёр игрока класса captain в ячейке (0, 0)", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 10
 testRunner.When(string.Format("Я перемещаю персонажа на {0} клетку", moveDistance), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 11
 testRunner.Then(string.Format("Значение {0} стало {1}", stat, expectedValue), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 12
 testRunner.And(string.Format("Актёр под эффектом {0}", effect), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Наступление выживальных состояний (жажда/голод/утомление), Variant 0", new string[] {
                "survival",
                "dev0"}, SourceLine=15)]
        public virtual void НаступлениеВыживальныхСостоянийЖаждаГолодУтомление_Variant0()
        {
#line 7
this.НаступлениеВыживальныхСостоянийЖаждаГолодУтомление("50", "сытость", "0", "Слабый голод", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Наступление выживальных состояний (жажда/голод/утомление), Variant 1", new string[] {
                "survival",
                "dev0"}, SourceLine=15)]
        public virtual void НаступлениеВыживальныхСостоянийЖаждаГолодУтомление_Variant1()
        {
#line 7
this.НаступлениеВыживальныхСостоянийЖаждаГолодУтомление("100", "сытость", "-50", "Голод", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Наступление выживальных состояний (жажда/голод/утомление), Variant 2", new string[] {
                "survival",
                "dev0"}, SourceLine=15)]
        public virtual void НаступлениеВыживальныхСостоянийЖаждаГолодУтомление_Variant2()
        {
#line 7
this.НаступлениеВыживальныхСостоянийЖаждаГолодУтомление("150", "сытость", "-100", "Голодание", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Наступление выживальных состояний (жажда/голод/утомление), Variant 3", new string[] {
                "survival",
                "dev0"}, SourceLine=15)]
        public virtual void НаступлениеВыживальныхСостоянийЖаждаГолодУтомление_Variant3()
        {
#line 7
this.НаступлениеВыживальныхСостоянийЖаждаГолодУтомление("50", "вода", "0", "Слабая жажда", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Наступление выживальных состояний (жажда/голод/утомление), Variant 4", new string[] {
                "survival",
                "dev0"}, SourceLine=15)]
        public virtual void НаступлениеВыживальныхСостоянийЖаждаГолодУтомление_Variant4()
        {
#line 7
this.НаступлениеВыживальныхСостоянийЖаждаГолодУтомление("100", "вода", "-50", "Жажда", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Наступление выживальных состояний (жажда/голод/утомление), Variant 5", new string[] {
                "survival",
                "dev0"}, SourceLine=15)]
        public virtual void НаступлениеВыживальныхСостоянийЖаждаГолодУтомление_Variant5()
        {
#line 7
this.НаступлениеВыживальныхСостоянийЖаждаГолодУтомление("150", "вода", "-100", "Обезвоживание", ((string[])(null)));
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
