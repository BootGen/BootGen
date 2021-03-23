
import { CRC32 } from 'crc_32_ts';

export interface UndoStackFrame {
    crc32: number;
    content: string;
}

export class UndoStack {
    private data = Array<UndoStackFrame>();
    public length(): number{
        return this.data.length
    }
    public top(): UndoStackFrame|null {
        if (this.data.length == 0)
            return null;
        return this.data[this.data.length-1];
    }
    public pop() {
        this.data.pop();
    }
    public push(content: string) {
        this.data.push({
            content: content,
            crc32: CRC32.str(content)
        });
    }
}