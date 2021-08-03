export function toPascalCase(str: string): string {
    return str
        .replace(/\s(.)/g, function($1) { return $1.toUpperCase(); })
        .replace(/\s/g, '')
        .replace(/^(.)/, function($1) { return $1.toUpperCase(); });
}

export function toCamelCase(str: string): string {
    return str
        .replace(/\s(.)/g, function($1) { return $1.toUpperCase(); })
        .replace(/\s/g, '')
        .replace(/^(.)/, function($1) { return $1.toLowerCase(); });
}

export function getJsonLength (json: string): number{
    json = json.replace(/ {2}/g, '');
    json = json.replace(/": /g, '":');
    json = json.replace(/[\n\t\r]/g, '');
    return json.length;
}
