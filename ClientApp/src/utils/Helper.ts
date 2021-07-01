export function delay(ms: number): Promise<void> {
    return new Promise(resolve => setTimeout(resolve, ms));
}

export function toCamelCase(str: string): string {
    const nameSpace = str.replace(/(?:^\w|[A-Z]|\b\w)/g, function(word, index) {
        return index === 0 ? word.toLowerCase() : word.toUpperCase();
    }).replace(/\s+/g, '');
    return `${nameSpace.charAt(0).toUpperCase()}${nameSpace.slice(1)}`;
}