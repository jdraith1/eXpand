using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using Xpand.ExpressApp.Logic.Model;

namespace Xpand.ExpressApp.Logic{
    public abstract class LogicRuleViewController<TModelLogicRule> : ViewController where TModelLogicRule : ILogicRule{

        private bool isRefreshing;
        public static readonly string ActiveObjectTypeHasRules = "ObjectTypeHas" + typeof(TModelLogicRule).Name;

        public virtual bool IsReady{
            get { return Active.ResultValue && View != null && View.ObjectTypeInfo != null; }
        }

        public virtual void ForceExecution(bool isReady, View view, bool invertCustomization, ExecutionContext executionContext){
            if (isReady) {
                object currentObject = view.CurrentObject;
                var modelLogicRules = LogicRuleManager<TModelLogicRule>.Instance[view.ObjectTypeInfo].Where(rule => IsValidRule(rule, view));
                IEnumerable<LogicRuleInfo<TModelLogicRule>> logicRuleInfos = GetContextValidLogicRuleInfos(view, modelLogicRules, currentObject, executionContext, invertCustomization);
                foreach (var logicRuleInfo in logicRuleInfos) {
                    ForceExecutionCore(logicRuleInfo,executionContext);
                }
            }
        }

        protected virtual IEnumerable<LogicRuleInfo<TModelLogicRule>> GetContextValidLogicRuleInfos(View view, IEnumerable<TModelLogicRule> modelLogicRules, object currentObject, ExecutionContext executionContext, bool invertCustomization) {
            return modelLogicRules.Select(rule => GetInfo(view, currentObject, rule, executionContext, invertCustomization)).Where(info => info!=null);
        }

        void ForceExecutionCore(LogicRuleInfo<TModelLogicRule> logicRuleInfo, ExecutionContext executionContext) {
            var args = new LogicRuleExecutingEventArgs<TModelLogicRule>(logicRuleInfo, false, executionContext);
            OnLogicRuleExecuting(args);
            if (!args.Cancel){
                ExecuteRule(logicRuleInfo, executionContext);
            }
            OnLogicRuleExecuted(new LogicRuleExecutedEventArgs<TModelLogicRule>(logicRuleInfo, executionContext));
        }

        LogicRuleInfo<TModelLogicRule> GetInfo(View view, object currentObject, TModelLogicRule rule, ExecutionContext executionContext, bool invertCustomization) {
            LogicRuleInfo<TModelLogicRule> info = CalculateLogicRuleInfo(currentObject, rule);
            if (info != null && ExecutionContextIsValid(executionContext, info)&&TemplateContextIsValid(info)&&ViewIsRoot(info)){
                info.InvertingCustomization = invertCustomization;
                info.View = view;
                if (invertCustomization)
                    info.Active = !info.Active;
                return info;
            }
            return null;
        }

        protected virtual bool ViewIsRoot(LogicRuleInfo<TModelLogicRule> info) {
            if (!(info.Rule.IsRootView.HasValue))
                return true;
            return info.Rule.IsRootView==View.IsRoot;
        }

        protected virtual bool TemplateContextIsValid(LogicRuleInfo<TModelLogicRule> info) {
            FrameTemplateContext frameTemplateContext = info.Rule.FrameTemplateContext;
            if (frameTemplateContext==FrameTemplateContext.All)
                return true;
            return frameTemplateContext+"Context" == Frame.Context;
        }


        public abstract void ExecuteRule(LogicRuleInfo<TModelLogicRule> info, ExecutionContext executionContext);


        
        public event EventHandler<LogicRuleExecutingEventArgs<TModelLogicRule>> LogicRuleExecuting;

        
        public event EventHandler<LogicRuleExecutedEventArgs<TModelLogicRule>> LogicRuleExecuted;
        

        private void FrameOnViewChanging(object sender, EventArgs args){
            if (View != null) InvertExecution(View);
            var view = ((ViewChangingEventArgs)args).View;
            Active[ActiveObjectTypeHasRules] = LogicRuleManager<TModelLogicRule>.HasRules(view);
            ForceExecution(Active[ActiveObjectTypeHasRules] && view != null && view.ObjectTypeInfo != null, view, false, ExecutionContext.ViewChanging);
            
        }

        private void InvertExecution(View view){
            ForceExecution(Active[ActiveObjectTypeHasRules] && view != null && view.ObjectTypeInfo != null, view,true, ExecutionContext.ViewChanging);
        }

        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            Frame.ViewChanging += FrameOnViewChanging;
            Frame.TemplateChanged+=FrameOnTemplateChanged;
        }

        void FrameOnTemplateChanged(object sender, EventArgs eventArgs) {
            var supportViewControlAdding = (Frame.Template) as ISupportViewControlAdding;
            if (supportViewControlAdding != null)
                supportViewControlAdding.ViewControlAdding += (o, args) => ForceExecution(ExecutionContext.ViewControlAdding); 
        }

        protected override void OnActivated(){base.OnActivated();
            if (IsReady){
                ObjectSpace.Committed+=ObjectSpaceOnCommitted;
                Frame.TemplateViewChanged += FrameOnTemplateViewChanged;
                ForceExecution(ExecutionContext.ControllerActivated);
                View.ObjectSpace.ObjectChanged += ObjectSpaceOnObjectChanged;
                View.CurrentObjectChanged+=ViewOnCurrentObjectChanged;
                View.ObjectSpace.Refreshing += ObjectSpace_Refreshing;
                View.ObjectSpace.Reloaded += ObjectSpace_Reloaded;
                if (View is XpandListView)
                    Frame.GetController<ListViewProcessCurrentObjectController>().CustomProcessSelectedItem+=OnCustomProcessSelectedItem;
            }
        }

        void OnCustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs customProcessListViewSelectedItemEventArgs) {
            ForceExecution(ExecutionContext.CustomProcessSelectedItem);
        }


        void ObjectSpaceOnCommitted(object sender, EventArgs eventArgs) {
            ForceExecution(ExecutionContext.ObjectSpaceCommited);
        }

        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            ForceExecution(ExecutionContext.ViewControlsCreated);
        }
        protected override void OnDeactivating(){
            base.OnDeactivating();
            if (IsReady){
                ObjectSpace.Committed -= ObjectSpaceOnCommitted;
                Frame.TemplateViewChanged-=FrameOnTemplateViewChanged;
                View.ObjectSpace.ObjectChanged -= ObjectSpaceOnObjectChanged;
                View.CurrentObjectChanged -= ViewOnCurrentObjectChanged;
                View.ObjectSpace.Refreshing -= ObjectSpace_Refreshing;
                View.ObjectSpace.Reloaded -= ObjectSpace_Reloaded;
                if (View is XpandListView)
                    Frame.GetController<ListViewProcessCurrentObjectController>().CustomProcessSelectedItem -= OnCustomProcessSelectedItem;
            }
        }

        void FrameOnTemplateViewChanged(object sender, EventArgs eventArgs) {
            ForceExecution(ExecutionContext.TemplateViewChanged);
        }

        private void ObjectSpace_Reloaded(object sender, EventArgs e){
            isRefreshing = false;
            ForceExecution(ExecutionContext.ObjectSpaceReloaded);
        }

        private void ObjectSpace_Refreshing(object sender, CancelEventArgs e){
            isRefreshing = true;
        }

        private void ViewOnCurrentObjectChanged(object sender, EventArgs args){
            if (!isRefreshing){
                ForceExecution(ExecutionContext.CurrentObjectChanged);
            }
        }

        private void ForceExecution(ExecutionContext executionContext,View view) {
            ForceExecution(IsReady, view, false, executionContext);
        }

        private void ForceExecution(ExecutionContext executionContext){
            ForceExecution(executionContext,View);
        }

        private void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs args){
            if (!String.IsNullOrEmpty(args.PropertyName)){
                ForceExecution(ExecutionContext.ObjectChanged);
            }
        }

        protected virtual void OnLogicRuleExecuting(LogicRuleExecutingEventArgs<TModelLogicRule> args){
            if (LogicRuleExecuting != null){
                LogicRuleExecuting(this, args);
            }
        }

        protected virtual void OnLogicRuleExecuted(LogicRuleExecutedEventArgs<TModelLogicRule> args){
            if (LogicRuleExecuted != null){
                LogicRuleExecuted(this, args);
            }
        }

        protected virtual bool IsValidRule(TModelLogicRule rule, View view){
            return view != null &&(IsValidViewId(view, rule))&&
                   IsValidViewType(view, rule) && IsValidNestedType(rule, view) && IsValidTypeInfo(view, rule);
        }

        bool IsValidViewId(View view, TModelLogicRule rule) {
            return rule.View == null|| view.Id== rule.View.Id;
        }

        bool IsValidTypeInfo(View view, TModelLogicRule rule) {
            return ((rule.TypeInfo != null && rule.TypeInfo.IsAssignableFrom(view.ObjectTypeInfo)) || rule.TypeInfo == null);
        }

        private bool IsValidNestedType(TModelLogicRule rule, View view) {
            return view is XpandDetailView || (rule.Nesting == Nesting.Any || view.IsRoot);
        }

        private bool IsValidViewType(View view, TModelLogicRule rule){
            return (rule.ViewType == ViewType.Any || (view is XpandDetailView ? rule.ViewType == ViewType.DetailView : rule.ViewType==ViewType.ListView));
        }

        protected virtual LogicRuleInfo<TModelLogicRule> CalculateLogicRuleInfo(object targetObject, TModelLogicRule modelLogicRule){
            var logicRuleInfo = (LogicRuleInfo<TModelLogicRule>)Activator.CreateInstance(typeof(LogicRuleInfo<TModelLogicRule>));
            logicRuleInfo.Active = true;
            logicRuleInfo.Object = targetObject;
            logicRuleInfo.Rule = modelLogicRule;

            logicRuleInfo.ExecutionContext = CalculateCurrentExecutionContext(modelLogicRule.ExecutionContextGroup);
            return logicRuleInfo;
        }

        ExecutionContext CalculateCurrentExecutionContext(string executionContextGroup) {
            var modelGroupContexts = GetModelGroupContexts(executionContextGroup);
            IModelExecutionContexts executionContexts = modelGroupContexts.Where(context => context.Id == executionContextGroup).Single();
            return executionContexts.Aggregate(ExecutionContext.None, (current, modelGroupContext) =>
                                                current | (ExecutionContext)Enum.Parse(typeof(ExecutionContext), modelGroupContext.Name.ToString()));    
        }

        protected abstract IModelGroupContexts GetModelGroupContexts(string executionContextGroup);
        



        public virtual bool ExecutionContextIsValid(ExecutionContext executionContext, LogicRuleInfo<TModelLogicRule> logicRuleInfo) {
            return (logicRuleInfo.ExecutionContext | executionContext) == logicRuleInfo.ExecutionContext;
        }
    }
}