export function startViewTransition(component, methodName) {
    console.log('Run JS start transition');
    if (!document.startViewTransition) {
        component.invokeMethodAsync(methodName);
        return;
    }
    console.log('startTransition exists');
    // With a transition:
    document.startViewTransition(async () => {
        console.log('call dotnet method to run needed code...');
        await component.invokeMethodAsync(methodName);
    });
}

export function isSupported() {
    return !!document.startViewTransition;
}